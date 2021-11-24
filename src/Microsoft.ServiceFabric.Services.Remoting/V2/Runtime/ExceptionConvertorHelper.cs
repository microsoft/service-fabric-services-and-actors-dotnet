// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
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

        public ServiceException ToServiceException(Exception originalException)
        {
            ServiceException serviceException = null;
            foreach (var convertor in this.convertors)
            {
                try
                {
                    if (convertor.TryConvertToServiceException(originalException, out serviceException))
                    {
                        var innerEx = convertor.GetInnerExceptions(originalException); // TODO limit the recursion to a degree
                        if (innerEx != null && innerEx.Length > 0)
                        {
                            serviceException.ActualInnerExceptions = new List<ServiceException>();
                            foreach (var inner in innerEx)
                            {
                                serviceException.ActualInnerExceptions.Add(this.ToServiceException(inner));
                            }
                        }

                        break;
                    }
                }
                catch (Exception)
                {
                    // Throw
                }
            }

            return serviceException;
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

        public List<ArraySegment<byte>> SerializeRemoteException(RemoteException2Wrapper remoteException2Wrapper)
        {
            var serializer = new DataContractSerializer(
                typeof(RemoteException2Wrapper),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = new List<Type>() { typeof(RemoteException2) },
                });

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    serializer.WriteObject(writer, remoteException2Wrapper);
                    writer.Flush();
                    return new List<ArraySegment<byte>>()
                    {
                       new ArraySegment<byte>(stream.ToArray()),
                    };
                }
            }
        }

        public class DefaultExceptionConvetor : IExceptionConvertor
        {
            public Exception[] GetInnerExceptions(Exception exception)
            {
               return exception.InnerException == null ? null : new Exception[] { exception.InnerException };
            }

            public bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
            {
                serviceException = new ServiceException(originalException.GetType().ToString(), originalException.Message);
                serviceException.ActualExceptionStackTrace = originalException.StackTrace;
                serviceException.ActualExceptionData = new Dictionary<object, object>()
                {
                    { "HResult", originalException.HResult },
                };

                if (originalException is FabricException fabricEx)
                {
                    serviceException.ActualExceptionData.Add("FabricErrorCode", fabricEx.ErrorCode);
                }

                return true;
            }
        }
    }
}
