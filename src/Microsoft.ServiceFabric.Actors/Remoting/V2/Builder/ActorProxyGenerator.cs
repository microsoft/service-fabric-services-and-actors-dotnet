// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    using System;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    internal class ActorProxyGenerator : ProxyGenerator
    {
        private readonly IProxyActivator proxyActivator;

        public ActorProxyGenerator(
            Type proxyInterfaceType,
            IProxyActivator proxyActivator)
            : base(proxyInterfaceType)
        {
            this.proxyActivator = proxyActivator;
        }

        public ActorProxy CreateActorProxy(
            ActorServicePartitionClient remotingPartitionClient,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory)
        {
            var serviceProxy = (ActorProxy)this.proxyActivator.CreateInstance();
            serviceProxy.Initialize(remotingPartitionClient, remotingMessageBodyFactory);
            return serviceProxy;
        }
    }
}
