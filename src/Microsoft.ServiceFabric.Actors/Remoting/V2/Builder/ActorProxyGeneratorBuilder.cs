// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    using System;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;

    internal class ActorProxyGeneratorBuilder : Services.Remoting.Base.V2.Builder.ProxyGeneratorBuilder<ActorProxyGenerator, ActorProxy>
    {
        public ActorProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
        }

        protected override ActorProxyGenerator CreateProxyGenerator(
            Type proxyInterfaceType,
            Type proxyActivatorType)
        {
            return new ActorProxyGenerator(
                proxyInterfaceType,
                (IProxyActivator)Activator.CreateInstance(proxyActivatorType));
        }
    }
}
