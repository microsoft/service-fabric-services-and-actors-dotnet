// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    /// Specifies the headers that are sent along with a ServiceRemoting message.
    /// </summary>
    public interface IActorRemotingMessageHeaders : IServiceRemotingRequestMessageHeader
    {
        /// <summary>
        /// This is the actorId to which remoting request will dispatch to.
        /// </summary>
        ActorId ActorId { get; set; }

        /// <summary>
        /// This is used to limit ren-entrancy in Actors.
        /// </summary>
        string CallContext { get; set; }
    }
}
