// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal class FabricActorExceptionConvertor : ExceptionConvertorBase
    {
        public override Exception[] GetInnerExceptions(Exception originalException)
        {
            return FabricActorExceptionKnownTypes.ServiceExceptionConvertors[originalException.GetType().ToString()].InnerExFunc(originalException as FabricException);
        }

        public override bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
        {
            serviceException = null;
            if (originalException is FabricException && FabricActorExceptionKnownTypes.ServiceExceptionConvertors.TryGetValue(originalException.GetType().ToString(), out var func))
            {
                serviceException = func.ToServiceExFunc(originalException as FabricException);

                return true;
            }

            return false;
        }
    }
}
