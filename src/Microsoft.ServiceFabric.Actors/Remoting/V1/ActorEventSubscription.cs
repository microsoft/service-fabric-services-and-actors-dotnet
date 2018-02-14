// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1
{
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Services.Common;

    internal static class ActorEventSubscription
    {
        public static readonly DataContractSerializer Serializer;
        public static readonly int InterfaceId;
        public static readonly int SubscribeMethodId;
        public static readonly int UnSubscribeMethodId;

        static ActorEventSubscription()
        {
            Serializer =
                ActorMessageBodySerializer.GetActorMessageSerializer(new[] { typeof(EventSubscriptionRequestBody) });
            InterfaceId = IdUtil.ComputeId("IActorEventSubscription", "System.Fabric.Actors.Communication");
            SubscribeMethodId = "SubscribeAsyc".GetHashCode();
            UnSubscribeMethodId = "UnSubscribeAsyc".GetHashCode();
        }
    }
}
