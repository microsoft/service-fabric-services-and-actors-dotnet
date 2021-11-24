// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.ServiceFabric.Services.Communication;

    internal class ExceptionConvertorHelper
    {
        private IEnumerable<IExceptionConvertor> convertors;

        public ExceptionConvertorHelper(IEnumerable<IExceptionConvertor> convertors)
        {
            this.convertors = convertors;
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
                catch (Exception)
                {
                    // Throw
                }
            }

            return actualEx != null ? actualEx : serviceException;
        }

        public List<ServiceException> FromRemoteExceptionWrapper(RemoteException2Wrapper exWrapper)
        {
            var svcExList = new List<ServiceException>();
            foreach (var remoteEx in exWrapper.Exceptions)
            {
                svcExList.Add(this.FromRemoteException(remoteEx));
            }

            return svcExList;
        }

        public ServiceException FromRemoteException(RemoteException2 remoteEx)
        {
            var svcEx = new ServiceException(remoteEx.Type, remoteEx.Message);
            svcEx.ActualExceptionStackTrace = remoteEx.StackTrace;
            svcEx.ActualExceptionData = remoteEx.Data;

            if (remoteEx.InnerExceptions != null && remoteEx.InnerExceptions.Count > 0)
            {
                svcEx.ActualInnerExceptions = new List<ServiceException>();
                foreach (var inner in remoteEx.InnerExceptions)
                {
                    svcEx.ActualInnerExceptions.Add(this.FromRemoteException(inner));
                }
            }

            return svcEx;
        }

        public bool TryDeserializeRemoteException(Stream stream, out RemoteException2Wrapper exWrapper)
        {
            exWrapper = null;
            var serializer = new DataContractSerializer(
                typeof(RemoteException2Wrapper),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = new List<Type>() { typeof(RemoteException2) },
                });

            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
                {
                    exWrapper = (RemoteException2Wrapper)serializer.ReadObject(reader);

                    return true;
                }
            }
            catch (Exception)
            {
                // Throw
            }

            return false;
        }
    }
}
