// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    internal class ExceptionConversionHandler
    {
        private static readonly string TraceEventType = "ExceptionConversionHandler";
        private IEnumerable<IExceptionConvertor> convertors;
        private FabricTransportRemotingSettings remotingSettings;

        public ExceptionConversionHandler(IEnumerable<IExceptionConvertor> convertors, FabricTransportRemotingSettings remotingSettings)
        {
            this.convertors = convertors;
            this.remotingSettings = remotingSettings;
        }

        public Exception FromServiceException(ServiceException serviceException)
        {
            List<Exception> innerExceptions = new List<Exception>();
            if (serviceException.ActualInnerExceptions != null && serviceException.ActualInnerExceptions.Count > 0)
            {
                foreach (var inner in serviceException.ActualInnerExceptions)
                {
                    innerExceptions.Add(this.FromServiceException(inner));
                }
            }

            Exception actualEx = null;
            foreach (var convertor in this.convertors)
            {
                try
                {
                    if (innerExceptions.Count == 0)
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, out actualEx))
                        {
                            break;
                        }
                    }
                    else if (innerExceptions.Count == 1)
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, innerExceptions[0], out actualEx))
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, innerExceptions.ToArray(), out actualEx))
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServiceTrace.Source.WriteWarning(
                        TraceEventType,
                        "Failed to convert ServiceException({0}) to ActualException : Reason - {1}",
                        serviceException.ActualExceptionType,
                        ex);
                }
            }

            return actualEx != null ? actualEx : serviceException;
        }

        public ServiceException FromRemoteException2(RemoteException2 remoteEx)
        {
            var svcEx = new ServiceException(remoteEx.Type, remoteEx.Message);
            svcEx.ActualExceptionStackTrace = remoteEx.StackTrace;
            svcEx.ActualExceptionData = remoteEx.Data;

            if (remoteEx.InnerExceptions != null && remoteEx.InnerExceptions.Count > 0)
            {
                svcEx.ActualInnerExceptions = new List<ServiceException>();
                foreach (var inner in remoteEx.InnerExceptions)
                {
                    svcEx.ActualInnerExceptions.Add(this.FromRemoteException2(inner));
                }
            }

            return svcEx;
        }

        public RemoteException2 DeserializeRemoteException2(byte[] buffer)
        {
            var serializer = new DataContractSerializer(
                typeof(RemoteException2),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                });

            try
            {
                using (var reader = XmlDictionaryReader.CreateBinaryReader(buffer, XmlDictionaryReaderQuotas.Max))
                {
                    return (RemoteException2)serializer.ReadObject(reader);
                }
            }
            catch (Exception e)
            {
                ServiceTrace.Source.WriteWarning(
                    TraceEventType,
                    "Failed to deserialize stream to RemoteException2: Reason - {0}",
                    e);

                throw e;
            }
        }

        public async Task DeserializeRemoteExceptionAndThrowAsync(Stream stream)
        {
            Exception exceptionToThrow = null;

            // Workaround as NativeMessageStream doesn't suport multi read.
            var streamLength = stream.Length;
            var buffer = new byte[streamLength];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            try
            {
                var remoteException2 = this.DeserializeRemoteException2(buffer);
                var svcEx = this.FromRemoteException2(remoteException2);
                var ex = this.FromServiceException(svcEx);

                if (ex is AggregateException)
                {
                    exceptionToThrow = ex;
                }
                else
                {
                    exceptionToThrow = new AggregateException(ex);
                }
            }
            catch (Exception dcsE)
            {
                exceptionToThrow = new ServiceException(dcsE.GetType().FullName, 
                    string.Format(CultureInfo.InvariantCulture, SR.ErrorDeserializationFailure, dcsE.ToString()));
            }

            var requestId = LogContext.GetRequestIdOrDefault();
            ServiceTrace.Source.WriteInfo(
                TraceEventType,
                "[{0}] Remoting call failed with exception : {1}",
                requestId,
                exceptionToThrow);

            throw exceptionToThrow;
        }
    }
}
