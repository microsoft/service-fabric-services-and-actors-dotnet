// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for triggering partition's backup.
    /// </summary>
    public partial class BackupPartitionDescription
    {
        /// <summary>
        /// Initializes a new instance of the BackupPartitionDescription class.
        /// </summary>
        /// <param name="backupStorage">Specifies the details of the backup storage where to save the backup.</param>
        public BackupPartitionDescription(
            BackupStorageDescription backupStorage = default(BackupStorageDescription))
        {
            this.BackupStorage = backupStorage;
        }

        /// <summary>
        /// Gets specifies the details of the backup storage where to save the backup.
        /// </summary>
        public BackupStorageDescription BackupStorage { get; }
    }
}
