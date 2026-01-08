// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ReconfigurationPhase.
    /// </summary>
    public enum ReconfigurationPhase
    {
        /// <summary>
        /// Indicates the invalid reconfiguration phase.
        /// </summary>
        Unknown,

        /// <summary>
        /// Specifies that there is no reconfiguration in progress.
        /// </summary>
        None,

        /// <summary>
        /// Refers to the phase where the reconfiguration is transferring data from the previous primary to the new primary.
        /// </summary>
        Phase0,

        /// <summary>
        /// Refers to the phase where the reconfiguration is querying the replica set for the progress.
        /// </summary>
        Phase1,

        /// <summary>
        /// Refers to the phase where the reconfiguration is ensuring that data from the current primary is present in a
        /// majority of the replica set.
        /// </summary>
        Phase2,

        /// <summary>
        /// This phase is for internal use only.
        /// </summary>
        Phase3,

        /// <summary>
        /// This phase is for internal use only.
        /// </summary>
        Phase4,

        /// <summary>
        /// This phase is for internal use only.
        /// </summary>
        AbortPhaseZero,
    }
}
