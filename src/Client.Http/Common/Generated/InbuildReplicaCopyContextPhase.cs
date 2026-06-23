// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for InbuildReplicaCopyContextPhase.
    /// </summary>
    public enum InbuildReplicaCopyContextPhase
    {
        /// <summary>
        /// The primary establishes the replication connection to the secondary replica.
        /// </summary>
        EstablishConnection,

        /// <summary>
        /// The primary retrieves secondary copy context (for example epoch and last operation sequence number) to determine
        /// the required copy strategy.
        /// </summary>
        GetCopyContext,
    }
}
