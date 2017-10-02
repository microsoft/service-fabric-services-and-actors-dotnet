// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
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
        private Remoting.V1.Builder.ActorEventProxyGeneratorWith actorV1EventProxyGeneratorW;
#endif
        public ActorEventProxyGenerator(Type proxyInterfaceType,
            IProxyActivator proxyActivator
            ) : base(proxyInterfaceType)
        {
            this.proxyActivator = proxyActivator;
        }

#if !DotNetCoreClr
        internal void InitializeV1ProxyGenerator(Remoting.V1.Builder.ActorEventProxyGeneratorWith actorEventProxyGeneratorWith)
        {
            this.actorV1EventProxyGeneratorW = actorEventProxyGeneratorWith;
        }
#endif


        public ActorEventProxy CreateActorEventProxy()
        {
            var actorEventProxy = (ActorEventProxy)this.proxyActivator.CreateInstance();
#if !DotNetCoreClr
            actorEventProxy.Initialize(this.actorV1EventProxyGeneratorW);
#endif
            return actorEventProxy;
        }
    }
}