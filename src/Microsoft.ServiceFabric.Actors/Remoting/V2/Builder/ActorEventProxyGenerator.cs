// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    using System;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;

    internal class ActorEventProxyGenerator : ProxyGenerator
    {
        private readonly IProxyActivator proxyActivator;

#if !DotNetCoreClr
        [Obsolete("This field is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        private Remoting.V1.Builder.ActorEventProxyGeneratorWith actorV1EventProxyGeneratorW;
#endif

        public ActorEventProxyGenerator(
            Type proxyInterfaceType,
            IProxyActivator proxyActivator)
            : base(proxyInterfaceType)
        {
            this.proxyActivator = proxyActivator;
        }

        public ActorEventProxy CreateActorEventProxy()
        {
            var actorEventProxy = (ActorEventProxy)this.proxyActivator.CreateInstance();
#if !DotNetCoreClr
#pragma warning disable 618
            actorEventProxy.Initialize(this.actorV1EventProxyGeneratorW);
#pragma warning restore 618
#endif
            return actorEventProxy;
        }

#if !DotNetCoreClr
        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        internal void InitializeV1ProxyGenerator(Remoting.V1.Builder.ActorEventProxyGeneratorWith actorEventProxyGeneratorWith)
        {
            this.actorV1EventProxyGeneratorW = actorEventProxyGeneratorWith;
        }
#endif
    }
}
