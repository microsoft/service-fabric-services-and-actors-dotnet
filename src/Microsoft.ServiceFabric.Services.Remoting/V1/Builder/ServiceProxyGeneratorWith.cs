// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Client;

    internal class ServiceProxyGeneratorWith : ProxyGeneratorWithSerializer
    {
        private readonly IProxyActivator proxyActivator;

        public ServiceProxyGeneratorWith(
            Type serviceProxyInterfaceType,
            IProxyActivator proxyActivator,
            IDictionary<int, IEnumerable<Type>> requestBodyTypes, // interfaceId -> RequestBodyTypes
            IDictionary<int, IEnumerable<Type>> responseBodyTypes) // interfaceId -> ResponseBodyTypes
            : base(
                serviceProxyInterfaceType,
                GetBodySerializers(requestBodyTypes),
                GetBodySerializers(responseBodyTypes))
        {
            this.proxyActivator = proxyActivator;
        }

        public ServiceProxy CreateServiceProxy(ServiceRemotingPartitionClient remotingPartitionClient)
        {
            var serviceProxy = (ServiceProxy)this.proxyActivator.CreateInstance();
            serviceProxy.Initialize(this, remotingPartitionClient);

            return serviceProxy;
        }

        private static IDictionary<int, DataContractSerializer> GetBodySerializers(
            IDictionary<int, IEnumerable<Type>> bodyTypes)
        {
            return bodyTypes.ToDictionary(
                item => item.Key,
                item => ServiceRemotingMessageSerializer.GetMessageBodySerializer(item.Value));
        }
    }
}
