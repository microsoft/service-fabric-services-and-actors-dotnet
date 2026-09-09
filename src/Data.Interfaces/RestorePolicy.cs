// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    /// <summary>
    /// Specifies the policy applied when restoring a replica from backup.
    /// </summary>
    public enum RestorePolicy : int
    {
        /// <summary>
        /// Verifies that the backup being restored is ahead of the current state and fails the restore otherwise.
        /// </summary>
        Safe = 0,

        /// <summary>
        /// Restores the backup without verifying that it is ahead of the current state.
        /// </summary>
        Force = 1,
    }
}
