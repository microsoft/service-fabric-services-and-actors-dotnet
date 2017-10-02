// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    /// <summary>
    /// Specifies Reentrancy mode for actor method calls.
    /// </summary>
    public enum ActorReentrancyMode
    {
        /// <summary>
        /// Allows actors to be reentrant if they are in the same call context chain. This is the default option for actors.
        /// </summary>
        LogicalCallContext = 1,

        /// <summary>
        /// Disallows actors to be reentrant. In this case if an actor sends a reentrant message to another 
        /// actor, an exception of type <see cref="System.Fabric.FabricException"/> will be thrown.
        /// </summary>
        Disallowed = 2
    }
}
