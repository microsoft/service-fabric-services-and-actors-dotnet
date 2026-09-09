// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    /// <summary>
    /// Indicates the kind of backup.
    /// </summary>
    public enum BackupOption : int
    {
        /// <summary>
        /// Represents a full backup of the reliable state of the replica.
        /// </summary>
        Full = 0,

        /// <summary>
        /// Represents an incremental backup containing only the changes to the reliable state of the replica since the last <see cref="Full"/> or incremental backup.
        /// </summary>
        Incremental = 1,
    }
}
