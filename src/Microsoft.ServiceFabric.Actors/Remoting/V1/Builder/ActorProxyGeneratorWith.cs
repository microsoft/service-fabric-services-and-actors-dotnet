// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Actors.Remoting.V1.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Builder;

    internal class ActorProxyGeneratorWith : ProxyGeneratorWithSerializer
    {
        private readonly IProxyActivator proxyActivator;

        public ActorProxyGeneratorWith(
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

        public ActorProxy CreateActorProxy(ActorServicePartitionClient actorServicePartitionClient)
        {
            var actorProxy = (ActorProxy)this.proxyActivator.CreateInstance();
            actorProxy.Initialize(this, actorServicePartitionClient);

            return actorProxy;
        }

        private static IDictionary<int, DataContractSerializer> GetBodySerializers(
            IDictionary<int, IEnumerable<Type>> bodyTypes)
        {
            return bodyTypes.ToDictionary(
                item => item.Key,
                item => ActorMessageBodySerializer.GetActorMessageSerializer(item.Value));
        }
    }
}
