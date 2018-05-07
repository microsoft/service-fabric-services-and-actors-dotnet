// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    /// <summary>
    /// Indicates how actor state is stored for an actor service.
    /// </summary>
    public enum StatePersistence
    {
        /// <summary>
        /// No state is stored for the actor.
        /// </summary>
        None = 0,

        /// <summary>
        /// The actor state is kept in-memory only using a volatile state provider.
        /// </summary>
        Volatile = 1,

        /// <summary>
        /// The actor state is persisted to local disk using a persistent state provider.
        /// </summary>
        Persisted = 2,
    }
}
