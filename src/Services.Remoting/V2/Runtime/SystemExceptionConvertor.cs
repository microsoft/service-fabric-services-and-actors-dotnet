// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Services.Communication;

    internal class SystemExceptionConvertor : ExceptionConvertorBase
    {
        public SystemExceptionConvertor()
        {
        }

        public override Exception[] GetInnerExceptions(Exception originalException)
        {
            return SystemExceptionKnownTypes.ServiceExceptionConvertors[originalException.GetType().FullName].InnerExFunc.Invoke(originalException);
        }

        public override bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
        {
            serviceException = null;
            if (SystemExceptionKnownTypes.ServiceExceptionConvertors.TryGetValue(originalException.GetType().FullName, out var func))
            {
                serviceException = func.ToServiceExFunc(originalException);

                return true;
            }

            return false;
        }
    }
}
