// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication;

    internal class ExceptionConvertorHelper
    {
        private IList<IExceptionConvertor> convertors;

        public ExceptionConvertorHelper(IList<IExceptionConvertor> convertors)
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

            if (serviceException == null)
            {
                var defaultConveror = new DefaultExceptionConvetor();
                defaultConveror.TryConvertToServiceException(originalException, out serviceException);
                var innerEx = defaultConveror.GetInnerExceptions(originalException);
                if (innerEx != null && innerEx.Length > 0)
                {
                    serviceException.ActualInnerExceptions = new List<ServiceException>();
                    foreach (var inner in innerEx)
                    {
                        serviceException.ActualInnerExceptions.Add(this.ToServiceException(inner));
                    }
                }
            }

            return serviceException;
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
