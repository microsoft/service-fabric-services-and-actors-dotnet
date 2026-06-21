// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Services.Common;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Remoting;

public abstract class ActorMessageDispatchTest
{
    public sealed class InterfaceId : ActorMessageDispatchTest
    {
        [Fact]
        public void EqualsIdOfIActorCommunicationInterface() =>
            Assert.Equal(
                IdUtil.ComputeId("IActorCommunication", "Microsoft.ServiceFabric.Actors.Communication"),
                ActorMessageDispatch.InterfaceId);
    }

    public sealed class InterfaceIdV2 : ActorMessageDispatchTest
    {
        [Fact]
        public void EqualsCrcOfIActorCommunicationFullName() =>
            Assert.Equal(1034796820, ActorMessageDispatch.InterfaceIdV2);
    }
}
