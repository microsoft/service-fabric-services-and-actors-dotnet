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
        public ExceptionConvertorBase()
        {
        }

        public virtual Exception[] GetInnerExceptions(Exception originalException)
        {
            return originalException.InnerException == null ? null : new Exception[] { originalException.InnerException };
        }

        public abstract bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException);
    }
}
