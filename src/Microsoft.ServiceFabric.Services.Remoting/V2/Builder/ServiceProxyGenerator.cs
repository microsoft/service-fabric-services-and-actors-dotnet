// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Builder
{
    using System;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

    internal class ServiceProxyGenerator : ProxyGenerator
    {
        private readonly IProxyActivator proxyActivator;

        public ServiceProxyGenerator(Type type, IProxyActivator createInstance) : base(type)
        {
            this.proxyActivator = createInstance;
        }

        public ServiceProxy CreateServiceProxy
            (
                ServiceRemotingPartitionClient remotingPartitionClient,
            IServiceRemotingMessageBodyFactory remotingMessageBodyFactory)
        {
            var serviceProxy = (ServiceProxy)this.proxyActivator.CreateInstance();
            serviceProxy.Initialize(this, remotingPartitionClient, remotingMessageBodyFactory);
            return serviceProxy;
        }
    }
}
