// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Builder;

    internal class ActorEventProxyGeneratorWith : ProxyGeneratorWithSerializer
    {
        private readonly IProxyActivator proxyActivator;

        public ActorEventProxyGeneratorWith(
            Type actorEventInterfaceType,
            IProxyActivator proxyActivator,
            IDictionary<int, IEnumerable<Type>> requestBodyTypes, // interfaceId -> RequestBodyTypes
            IDictionary<int, IEnumerable<Type>> responseBodyTypes) // interfaceId -> ResponseBodyTypes)
            : base(actorEventInterfaceType, GetBodySerializers(requestBodyTypes), GetBodySerializers(responseBodyTypes))
        {
            this.proxyActivator = proxyActivator;
        }

        public ActorEventProxy CreateActorEventProxy()
        {
            var actorEventProxy = (ActorEventProxy)this.proxyActivator.CreateInstance();
            actorEventProxy.Initialize(this);

            return actorEventProxy;
        }

        private static IDictionary<int, DataContractSerializer> GetBodySerializers(IDictionary<int, IEnumerable<Type>> bodyTypes)
        {
            return bodyTypes.ToDictionary(item => item.Key, item => ActorMessageBodySerializer.GetActorMessageSerializer(item.Value));
        }
    }
}
