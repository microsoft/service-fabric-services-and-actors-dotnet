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
        /// The primary is establishing a connection to the secondary replica. The value is 0.
        /// </summary>
        EstablishConnection,

        /// <summary>
        /// The primary is retrieving copy context from the secondary to determine what type of copy is needed. The value is 1.
        /// </summary>
        GetCopyContext,
    }
}
