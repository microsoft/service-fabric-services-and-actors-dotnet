// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for InbuildReplicaPhase.
    /// </summary>
    public enum InbuildReplicaPhase
    {
        /// <summary>
        /// The primary establishes a connection to the secondary and retrieves its current state. The value is 0.
        /// </summary>
        CopyContext,

        /// <summary>
        /// The primary obtains its own current state in preparation for the copy. The value is 1.
        /// </summary>
        CopyState,

        /// <summary>
        /// The primary transfers state data to the secondary. The value is 2.
        /// </summary>
        Copy,

        /// <summary>
        /// The secondary applies replication operations it received from the primary during the Copy phase to finish catching
        /// up. The value is 3.
        /// </summary>
        CopyCatchup,

        /// <summary>
        /// The secondary replica is fully built and ready to transition to an idle secondary. The value is 4.
        /// </summary>
        CopyComplete,
    }
}
