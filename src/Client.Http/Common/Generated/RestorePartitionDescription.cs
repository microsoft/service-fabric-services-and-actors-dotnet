// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specifies the parameters needed to trigger a restore of a specific partition.
    /// </summary>
    public partial class RestorePartitionDescription
    {
        /// <summary>
        /// Initializes a new instance of the RestorePartitionDescription class.
        /// </summary>
        /// <param name="backupId">Unique backup ID.</param>
        /// <param name="backupLocation">Location of the backup relative to the backup storage specified/ configured.</param>
        /// <param name="backupStorage">Location of the backup from where the partition will be restored.</param>
        public RestorePartitionDescription(
            Guid? backupId,
            string backupLocation,
            BackupStorageDescription backupStorage = default(BackupStorageDescription))
        {
            backupId.ThrowIfNull(nameof(backupId));
            backupLocation.ThrowIfNull(nameof(backupLocation));
            this.BackupId = backupId;
            this.BackupLocation = backupLocation;
            this.BackupStorage = backupStorage;
        }

        /// <summary>
        /// Gets unique backup ID.
        /// </summary>
        public Guid? BackupId { get; }

        /// <summary>
        /// Gets location of the backup relative to the backup storage specified/ configured.
        /// </summary>
        public string BackupLocation { get; }

        /// <summary>
        /// Gets location of the backup from where the partition will be restored.
        /// </summary>
        public BackupStorageDescription BackupStorage { get; }
    }
}
