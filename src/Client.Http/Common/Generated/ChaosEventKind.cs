// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ChaosEventKind.
    /// </summary>
    public enum ChaosEventKind
    {
        /// <summary>
        /// Indicates an invalid Chaos event kind. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates a Chaos event that gets generated when Chaos is started.
        /// </summary>
        Started,

        /// <summary>
        /// Indicates a Chaos event that gets generated when Chaos has decided on the faults for an iteration. This Chaos event
        /// contains the details of the faults as a list of strings.
        /// </summary>
        ExecutingFaults,

        /// <summary>
        /// Indicates a Chaos event that gets generated when Chaos is waiting for the cluster to become ready for faulting, for
        /// example, Chaos may be waiting for the on-going upgrade to finish.
        /// </summary>
        Waiting,

        /// <summary>
        /// Indicates a Chaos event that gets generated when the cluster entities do not become stable and healthy within
        /// ChaosParameters.MaxClusterStabilizationTimeoutInSeconds.
        /// </summary>
        ValidationFailed,

        /// <summary>
        /// Indicates a Chaos event that gets generated when an unexpected event has occurred in the Chaos engine, for example,
        /// due to the cluster snapshot being inconsistent, while faulting a faultable entity Chaos found that the entity was
        /// already faulted.
        /// </summary>
        TestError,

        /// <summary>
        /// Indicates a Chaos event that gets generated when Chaos stops because either the user issued a stop or the time to
        /// run was up.
        /// </summary>
        Stopped,
    }
}
