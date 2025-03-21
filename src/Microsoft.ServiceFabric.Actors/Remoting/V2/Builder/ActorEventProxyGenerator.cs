// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Builder;

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    internal class ActorEventProxyGenerator : ProxyGenerator
    {
        private readonly IProxyActivator proxyActivator;

        public ActorEventProxyGenerator(
            Type proxyInterfaceType,
            IProxyActivator proxyActivator)
            : base(proxyInterfaceType)
        {
            this.proxyActivator = proxyActivator;
        }

        public ActorEventProxy CreateActorEventProxy()
        {
            return (ActorEventProxy)this.proxyActivator.CreateInstance();
        }
    }
}
