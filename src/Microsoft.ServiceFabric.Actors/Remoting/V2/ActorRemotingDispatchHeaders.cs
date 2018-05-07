// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    /// Specifies the headers that are sent along with a ServiceRemoting message. This class is used with Service Independent Dispatcher <see cref="ActorServiceRemotingDispatcher"/>
    /// .e.g Short-Circuiting (Where client and service are in same process)
    /// </summary>
    public class ActorRemotingDispatchHeaders : ServiceRemotingDispatchHeaders
    {
        /// <summary>
        /// Gets or sets the ActorId to which remoting request is dispatch to
        /// </summary>
        public ActorId ActorId { get; set; }

        /// <summary>
        /// Gets or sets the call context.
        /// This is used to limit re-entrancy in actors. This is an optional header. If not specified . It checks for existing callContext then it appends Guid to it and use it as a callContext for this request.
        /// If existing callContext is not present, it assigns random Guid to it.
        /// </summary>
        public string CallContext { get; set; }

        /// <summary>
        /// Gets or sets the Full Name for the user IActor  interface.
        /// </summary>
        public string ActorInterfaceName { get; set; }
    }
}
