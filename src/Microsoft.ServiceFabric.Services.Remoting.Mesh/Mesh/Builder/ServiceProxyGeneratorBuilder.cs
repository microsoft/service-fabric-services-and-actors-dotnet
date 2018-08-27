// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Mesh.Builder
{
    using System;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Mesh.Client;

    internal sealed class ServiceProxyGeneratorBuilder : Base.V2.Builder.ProxyGeneratorBuilder<ServiceProxyGenerator, ServiceProxy>
    {
        public ServiceProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
        }

        protected override ServiceProxyGenerator CreateProxyGenerator(Type proxyInterfaceType, Type proxyActivatorType)
        {
            return new ServiceProxyGenerator(
                proxyInterfaceType,
                (IProxyActivator)Activator.CreateInstance(proxyActivatorType));
        }
    }
}
