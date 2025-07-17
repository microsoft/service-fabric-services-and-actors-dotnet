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

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    sealed class ExceptionDeserializer
    {
        static readonly string TraceEventType = "ExceptionDeserializer";
        readonly IEnumerable<IExceptionConvertor> convertors;

        public ExceptionDeserializer(IEnumerable<IExceptionConvertor> convertors)
        {
            this.convertors = convertors;
        }

        Exception FromServiceException(ServiceException serviceException)
        {
            var innerExceptions = new List<Exception>();
            if (serviceException.ActualInnerExceptions != null && serviceException.ActualInnerExceptions.Count > 0)
            {
                foreach (ServiceException inner in serviceException.ActualInnerExceptions)
                    innerExceptions.Add(this.FromServiceException(inner));
            }

            Exception actualEx = null;
            foreach (IExceptionConvertor convertor in this.convertors)
            {
                try
                {
                    if (innerExceptions.Count == 0)
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, out actualEx))
                            break;
                    }
                    else if (innerExceptions.Count == 1)
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, innerExceptions[0], out actualEx))
                            break;
                    }
                    else
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, innerExceptions.ToArray(), out actualEx))
                            break;
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

        ServiceException FromRemoteException2(RemoteException2 remoteEx)
        {
            var svcEx = new ServiceException(remoteEx.Type, remoteEx.Message);
            svcEx.ActualExceptionStackTrace = remoteEx.StackTrace;
            svcEx.ActualExceptionData = remoteEx.Data;

            if (remoteEx.InnerExceptions != null && remoteEx.InnerExceptions.Count > 0)
            {
                svcEx.ActualInnerExceptions = new List<ServiceException>();
                foreach (var inner in remoteEx.InnerExceptions)
                    svcEx.ActualInnerExceptions.Add(this.FromRemoteException2(inner));
            }

            return svcEx;
        }

        RemoteException2 DeserializeRemoteException2(byte[] buffer)
        {
            var settings = new DataContractSerializerSettings { MaxItemsInObjectGraph = int.MaxValue };
            var serializer = new DataContractSerializer(typeof(RemoteException2), settings);

            try
            {
                using var reader = XmlDictionaryReader.CreateBinaryReader(buffer, XmlDictionaryReaderQuotas.Max);
                return (RemoteException2)serializer.ReadObject(reader);
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
            Exception exceptionToThrow;

            // Workaround as NativeMessageStream doesn't suport multi read.
            long streamLength = stream.Length;
            var buffer = new byte[streamLength];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            try
            {
                RemoteException2 remoteException2 = this.DeserializeRemoteException2(buffer);
                ServiceException svcEx = this.FromRemoteException2(remoteException2);
                Exception ex = this.FromServiceException(svcEx);
                exceptionToThrow = ex is AggregateException ? ex : new AggregateException(ex);
            }
            catch (Exception dcsE)
            {
                exceptionToThrow = new ServiceException(dcsE.GetType().FullName, 
                    string.Format(CultureInfo.InvariantCulture, SR.ErrorDeserializationFailure, dcsE.ToString()));
            }

            Guid requestId = LogContext.GetRequestIdOrDefault();
            ServiceTrace.Source.WriteInfo(
                TraceEventType,
                "[{0}] Remoting call failed with exception : {1}",
                requestId,
                exceptionToThrow);

            throw exceptionToThrow;
        }
    }
}
