// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    sealed class ExceptionSerializer
    {
        static readonly string TraceEventType = "ExceptionSerializer";
        readonly IEnumerable<IExceptionConvertor> convertors;
        readonly FabricTransportRemotingListenerSettings settings;

        public ExceptionSerializer(IEnumerable<IExceptionConvertor> convertors, FabricTransportRemotingListenerSettings settings)
        {
            this.convertors = convertors;
            this.settings = settings;
        }

        ServiceException ToServiceException(Exception originalException, int currentDepth)
        {
            ServiceException serviceException = null;
            foreach (IExceptionConvertor convertor in this.convertors)
            {
                try
                {
                    if (convertor.TryConvertToServiceException(originalException, out serviceException))
                    {
                        if (++currentDepth > this.settings.RemotingExceptionDepth)
                            break;

                        Exception[] innerExceptions = convertor.GetInnerExceptions(originalException);
                        if (innerExceptions != null && innerExceptions.Length > 0)
                        {
                            serviceException.ActualInnerExceptions = new List<ServiceException>();
                            int currentBreadth = 0;
                            foreach (Exception innerException in innerExceptions)
                            {
                                if (++currentBreadth > this.settings.RemotingExceptionDepth)
                                    break;

                                serviceException.ActualInnerExceptions.Add(this.ToServiceException(innerException, currentDepth));
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

        ServiceException ToServiceException(Exception originalException)
        {
            return this.ToServiceException(originalException, 1);
        }

        RemoteException2 ToRemoteException(ServiceException serviceException)
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
                    remoteException.InnerExceptions.Add(this.ToRemoteException(inner));
            }

            return remoteException;
        }

        List<ArraySegment<byte>> SerializeRemoteException(RemoteException2 remoteException)
        {
            var settings = new DataContractSerializerSettings { MaxItemsInObjectGraph = int.MaxValue };
            var serializer = new DataContractSerializer(typeof(RemoteException2), settings);

            using var stream = new MemoryStream();
            using var writer = XmlDictionaryWriter.CreateBinaryWriter(stream);
            serializer.WriteObject(writer, remoteException);
            writer.Flush();

            var segment = new ArraySegment<byte>(stream.ToArray());
            return new List<ArraySegment<byte>> { segment };
        }

        public List<ArraySegment<byte>> SerializeRemoteException(Exception exception)
        {
            ServiceException svcEx = this.ToServiceException(exception);
            RemoteException2 remoteEx = this.ToRemoteException(svcEx);
            return this.SerializeRemoteException(remoteEx);
        }
    }
}
