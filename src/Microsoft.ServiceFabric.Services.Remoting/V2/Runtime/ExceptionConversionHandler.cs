// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    internal class ExceptionConversionHandler
    {
        private static readonly string TraceEventType = "ExceptionConversionHandler";

        public static readonly int DefaultRemotingExceptionDepth = 2;
        private IEnumerable<IExceptionConvertor> convertors;
        private IExceptionSerializerSettings settings;

        internal ExceptionConversionHandler(IEnumerable<IExceptionConvertor> convertors, IExceptionSerializerSettings settings)
        {
            this.convertors = convertors;
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        internal static ExceptionConversionHandler CreateDefault(IEnumerable<IExceptionConvertor> exceptionConvertors, IExceptionSerializerSettings settings)
        {
            var convertors = new List<IExceptionConvertor>(exceptionConvertors ?? Enumerable.Empty<IExceptionConvertor>())
            {
                new SystemExceptionConvertor(),
                new FabricExceptionConvertor(),
                new DefaultExceptionConvertor()
            };

            return new ExceptionConversionHandler(convertors, settings);
        }

        public ServiceException ToServiceException(Exception originalException, int currentDepth)
        {
            ServiceException serviceException = null;
            foreach (var convertor in this.convertors)
            {
                try
                {
                    if (convertor.TryConvertToServiceException(originalException, out serviceException))
                    {
                        if (++currentDepth > this.settings.RemotingExceptionDepth)
                        {
                            break;
                        }

                        var innerEx = convertor.GetInnerExceptions(originalException);
                        if (innerEx != null && innerEx.Length > 0)
                        {
                            serviceException.ActualInnerExceptions = new List<ServiceException>();
                            int currentBreadth = 0;
                            foreach (var inner in innerEx)
                            {
                                if (++currentBreadth > this.settings.RemotingExceptionDepth)
                                {
                                    break;
                                }

                                serviceException.ActualInnerExceptions.Add(this.ToServiceException(inner, currentDepth));
                            }
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    ServiceTrace.Source.WriteWarning(
                       TraceEventType,
                       "Failed to convert ActualException({0}) to ServiceException : Reason - {1}",
                       originalException.GetType().Name,
                       ex);
                }
            }

            return serviceException;
        }

        public ServiceException ToServiceException(Exception originalException)
        {
            return this.ToServiceException(originalException, 1);
        }

        public RemoteException2 ToRemoteException(ServiceException serviceException)
        {
            var remoteException = new RemoteException2()
            {
                Message = serviceException.Message,
                Type = serviceException.ActualExceptionType,
                StackTrace = serviceException.ActualExceptionStackTrace,
                Data = serviceException.ActualExceptionData,
            };

            if (serviceException.ActualInnerExceptions != null && serviceException.ActualInnerExceptions.Count > 0)
            {
                remoteException.InnerExceptions = new List<RemoteException2>();
                foreach (var inner in serviceException.ActualInnerExceptions)
                {
                    remoteException.InnerExceptions.Add(this.ToRemoteException(inner));
                }
            }

            return remoteException;
        }

        public List<ArraySegment<byte>> SerializeRemoteException(RemoteException2 remoteException)
        {
            var serializer = new DataContractSerializer(
                typeof(RemoteException2),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                });

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    serializer.WriteObject(writer, remoteException);
                    writer.Flush();
                    return new List<ArraySegment<byte>>()
                    {
                       new ArraySegment<byte>(stream.ToArray()),
                    };
                }
            }
        }

        internal RemoteException2 BuildRemoteException(Exception exception)
        {
            ServiceException svcEx = this.ToServiceException(exception);
            return this.ToRemoteException(svcEx);
        }

        public List<ArraySegment<byte>> SerializeRemoteException(Exception exception)
        {
#pragma warning disable 618
            if (this.settings.ExceptionSerializationTechnique == FabricTransportRemotingListenerSettings.ExceptionSerialization.BinaryFormatter)
                return RemoteException.FromException(exception).Data;
#pragma warning restore 618

            RemoteException2 remoteEx = this.BuildRemoteException(exception);
            return this.SerializeRemoteException(remoteEx);
        }
    }
}
