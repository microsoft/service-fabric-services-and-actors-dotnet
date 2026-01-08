// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes additional filters to be applied, while listing backups, and backup storage details from where to fetch
    /// the backups.
    /// </summary>
    public partial class GetBackupByStorageQueryDescription
    {
        /// <summary>
        /// Initializes a new instance of the GetBackupByStorageQueryDescription class.
        /// </summary>
        /// <param name="storage">Describes the parameters for the backup storage from where to enumerate backups. This is
        /// optional and by default backups are enumerated from the backup storage where this backup entity is currently being
        /// backed up (as specified in backup policy). This parameter is useful to be able to enumerate backups from another
        /// cluster where you may intend to restore.</param>
        /// <param name="backupEntity">Indicates the entity for which to enumerate backups.</param>
        /// <param name="startDateTimeFilter">Specifies the start date time in ISO8601 from which to enumerate backups. If not
        /// specified, backups are enumerated from the beginning.</param>
        /// <param name="endDateTimeFilter">Specifies the end date time in ISO8601 till which to enumerate backups. If not
        /// specified, backups are enumerated till the end.</param>
        /// <param name="latest">If specified as true, gets the most recent backup (within the specified time range) for every
        /// partition under the specified backup entity.</param>
        public GetBackupByStorageQueryDescription(
            BackupStorageDescription storage,
            BackupEntity backupEntity,
            DateTime? startDateTimeFilter = default(DateTime?),
            DateTime? endDateTimeFilter = default(DateTime?),
            bool? latest = false)
        {
            storage.ThrowIfNull(nameof(storage));
            backupEntity.ThrowIfNull(nameof(backupEntity));
            this.Storage = storage;
            this.BackupEntity = backupEntity;
            this.StartDateTimeFilter = startDateTimeFilter;
            this.EndDateTimeFilter = endDateTimeFilter;
            this.Latest = latest;
        }

        /// <summary>
        /// Gets specifies the start date time in ISO8601 from which to enumerate backups. If not specified, backups are
        /// enumerated from the beginning.
        /// </summary>
        public DateTime? StartDateTimeFilter { get; }

        /// <summary>
        /// Gets specifies the end date time in ISO8601 till which to enumerate backups. If not specified, backups are
        /// enumerated till the end.
        /// </summary>
        public DateTime? EndDateTimeFilter { get; }

        /// <summary>
        /// Gets if specified as true, gets the most recent backup (within the specified time range) for every partition under
        /// the specified backup entity.
        /// </summary>
        public bool? Latest { get; }

        /// <summary>
        /// Gets the parameters for the backup storage from where to enumerate backups. This is optional and by default backups
        /// are enumerated from the backup storage where this backup entity is currently being backed up (as specified in
        /// backup policy). This parameter is useful to be able to enumerate backups from another cluster where you may intend
        /// to restore.
        /// </summary>
        public BackupStorageDescription Storage { get; }

        /// <summary>
        /// Gets indicates the entity for which to enumerate backups.
        /// </summary>
        public BackupEntity BackupEntity { get; }
    }
}
