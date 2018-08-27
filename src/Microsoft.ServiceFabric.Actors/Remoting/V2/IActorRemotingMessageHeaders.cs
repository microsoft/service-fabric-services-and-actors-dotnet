// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    /// Specifies the headers that are sent along with a ServiceRemoting message.
    /// </summary>
    public interface IActorRemotingMessageHeaders : IServiceRemotingRequestMessageHeader
    {
        /// <summary>
        /// Gets or sets the actorId to which remoting request will dispatch to.
        /// </summary>
        ActorId ActorId { get; set; }

        /// <summary>
        /// Gets or sets the call context which is used to limit ren-entrancy in Actors.
        /// </summary>
        string CallContext { get; set; }
    }
}
