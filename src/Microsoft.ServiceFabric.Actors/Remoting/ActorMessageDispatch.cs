// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting
{
    using Microsoft.ServiceFabric.Services.Common;

    internal static class ActorMessageDispatch
    {
        public static readonly int InterfaceId;
        public static readonly int InterfaceIdV2;

        static ActorMessageDispatch()
        {
            InterfaceId = IdUtil.ComputeId("IActorCommunication", "Microsoft.ServiceFabric.Actors.Communication");
            InterfaceIdV2 = IdUtil.ComputeIdWithCRC("Microsoft.ServiceFabric.Actors.Communication.IActorCommunication");
        }
    }
}
