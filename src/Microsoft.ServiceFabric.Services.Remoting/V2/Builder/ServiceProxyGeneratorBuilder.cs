// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Builder
{
    using System;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    internal sealed class ServiceProxyGeneratorBuilder : ProxyGeneratorBuilder<ServiceProxyGenerator, ServiceProxy>
    {
        public ServiceProxyGeneratorBuilder(ICodeBuilder codeBuilder) : base(codeBuilder)
        {
        }

        protected override ServiceProxyGenerator CreateProxyGenerator(
            Type proxyInterfaceType,
            Type proxyActivatorType)
        {
            return new ServiceProxyGenerator(
                proxyInterfaceType,
                (IProxyActivator)Activator.CreateInstance(proxyActivatorType));
        }
    }
}
