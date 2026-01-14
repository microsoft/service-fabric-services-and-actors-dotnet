// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for BackupState.
    /// </summary>
    public enum BackupState
    {
        /// <summary>
        /// Indicates an invalid backup state. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Operation has been validated and accepted. Backup is yet to be triggered.
        /// </summary>
        Accepted,

        /// <summary>
        /// Backup operation has been triggered and is under process.
        /// </summary>
        BackupInProgress,

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
