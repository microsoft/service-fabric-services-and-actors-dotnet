// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Runtime.InteropServices;
    using Microsoft.ServiceFabric.Services.Communication;

    /// <summary>
    /// Default convertor for FabricExceptions.
    /// </summary>
    internal class FabricExceptionConvertor : ExceptionConvertorBase
    {
        public FabricExceptionConvertor(IList<IExceptionConvertor> convertors)
            : base(convertors)
        {
        }

        public override Exception[] GetInnerExceptions(Exception originalException)
        {
            return FabricExceptionKnownTypes.ServiceExceptionConvertors[originalException.GetType().ToString()].InnerExFunc(originalException as FabricException);
        }

        public override bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
        {
            serviceException = null;
            if (originalException is FabricException && FabricExceptionKnownTypes.ServiceExceptionConvertors.TryGetValue(originalException.GetType().ToString(), out var func))
            {
                serviceException = func.ToServiceExFunc(originalException as FabricException);

                return true;
            }

            return false;
        }
    }
}
