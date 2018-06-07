// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime
{
    using Microsoft.ServiceFabric.Services.Common;

    internal class ActorEventSubscription
    {
        public static readonly string InterfaceName;
        public static readonly string SubscribeMethodName;
        public static readonly string UnSubscribeMethodName;
        public static readonly int InterfaceId;
        public static readonly int SubscribeMethodId;
        public static readonly int UnSubscribeMethodId;

        static ActorEventSubscription()
        {
            InterfaceName = "IActorEventSubscription";
            SubscribeMethodName = "SubscribeAsync";
            UnSubscribeMethodName = "UnSubscribeAsyc";
            InterfaceId = IdUtil.ComputeIdWithCRC(InterfaceName);
            SubscribeMethodId = IdUtil.ComputeIdWithCRC(SubscribeMethodName);
            UnSubscribeMethodId = IdUtil.ComputeIdWithCRC(UnSubscribeMethodName);
        }
    }
}
