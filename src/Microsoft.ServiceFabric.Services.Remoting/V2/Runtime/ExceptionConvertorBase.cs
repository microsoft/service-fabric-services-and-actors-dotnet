// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Services.Communication;

    internal abstract class ExceptionConvertorBase : IExceptionConvertor
    {
        private IList<IExceptionConvertor> convertors;

        public ExceptionConvertorBase(IList<IExceptionConvertor> convertors)
        {
            this.convertors = convertors;
        }

        public abstract IList<Exception> GetInnerExceptions(Exception originalException);

        public abstract ServiceException ToServiceException(Exception originalException);

        public abstract bool IsKnownType(Exception originalException);

        public bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
        {
            serviceException = null;
            if (this.IsKnownType(originalException))
            {
                var svcEx = this.ToServiceException(originalException);
                var innerExList = this.GetInnerExceptions(originalException);

                if (innerExList != null)
                {
                    svcEx.ActualInnerExceptions = new List<ServiceException>();
                    foreach (var innerEx in innerExList)
                    {
                        ServiceException innerSvcEx = null;
                        foreach (var convertor in this.convertors)
                        {
                            if (convertor.TryConvertToServiceException(innerEx, out innerSvcEx))
                            {
                                svcEx.ActualInnerExceptions.Add(innerSvcEx);
                                break;
                            }
                        }
                    }
                }

                serviceException = svcEx;

                return true;
            }

            return false;
        }
    }
}
