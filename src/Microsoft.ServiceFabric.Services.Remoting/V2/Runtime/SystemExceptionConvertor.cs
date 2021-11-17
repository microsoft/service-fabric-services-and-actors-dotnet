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
        public SystemExceptionConvertor(IList<IExceptionConvertor> convertors)
            : base(convertors)
        {
        }

        public override IList<Exception> GetInnerExceptions(Exception originalException)
        {
            return SystemExceptionKnownTypes.ServiceExceptionConvertors[originalException.GetType().ToString()].InnerExFunc.Invoke(originalException);
        }

        public override bool IsKnownType(Exception originalException)
        {
            return SystemExceptionKnownTypes.ServiceExceptionConvertors.ContainsKey(originalException.GetType().ToString());
        }

        public override ServiceException ToServiceException(Exception originalException)
        {
            var convFunc = SystemExceptionKnownTypes.ServiceExceptionConvertors[originalException.GetType().ToString()].ToServiceExFunc;

            return convFunc.Invoke(originalException);
        }
    }
}
