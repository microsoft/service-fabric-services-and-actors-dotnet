// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for RestoreState.
    /// </summary>
    public enum RestoreState
    {
        /// <summary>
        /// Indicates an invalid restore state. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Operation has been validated and accepted. Restore is yet to be triggered.
        /// </summary>
        Accepted,

        /// <summary>
        /// Restore operation has been triggered and is under process.
        /// </summary>
        RestoreInProgress,

        /// <summary>
        /// Restore operation for secondary replicas has been triggered and is under process.
        /// </summary>
        SecondariesInProgress,

        /// <summary>
        /// Operation completed with success.
        /// </summary>
        Success,

        /// <summary>
        /// Operation completed with failure.
        /// </summary>
        Failure,

        /// <summary>
        /// Operation timed out.
        /// </summary>
        Timeout,
    }
}
