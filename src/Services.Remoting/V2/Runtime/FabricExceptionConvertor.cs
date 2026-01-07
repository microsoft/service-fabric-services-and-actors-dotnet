// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication;

    /// <summary>
    /// Default convertor for FabricExceptions.
    /// </summary>
    internal class FabricExceptionConvertor : ExceptionConvertorBase
    {
        public FabricExceptionConvertor()
        {
        }

        public override Exception[] GetInnerExceptions(Exception originalException)
        {
            return FabricExceptionKnownTypes.ServiceExceptionConvertors[originalException.GetType().FullName].InnerExFunc(originalException as FabricException);
        }

        public override bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
        {
            serviceException = null;
            if (originalException is FabricException && FabricExceptionKnownTypes.ServiceExceptionConvertors.TryGetValue(originalException.GetType().FullName, out var func))
            {
                serviceException = func.ToServiceExFunc(originalException as FabricException);

                return true;
            }

            return false;
        }
    }
}
