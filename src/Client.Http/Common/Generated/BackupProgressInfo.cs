// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the progress of a partition's backup.
    /// </summary>
    public partial class BackupProgressInfo
    {
        /// <summary>
        /// Initializes a new instance of the BackupProgressInfo class.
        /// </summary>
        /// <param name="backupState">Represents the current state of the partition backup operation.
        /// . Possible values include: 'Invalid', 'Accepted', 'BackupInProgress', 'Success', 'Failure', 'Timeout'</param>
        /// <param name="timeStampUtc">TimeStamp in UTC when operation succeeded or failed.</param>
        /// <param name="backupId">Unique ID of the newly created backup.</param>
        /// <param name="backupLocation">Location, relative to the backup store, of the newly created backup.</param>
        /// <param name="epochOfLastBackupRecord">Specifies the epoch of the last record included in backup.</param>
        /// <param name="lsnOfLastBackupRecord">The LSN of last record included in backup.</param>
        /// <param name="failureError">Denotes the failure encountered in performing backup operation.</param>
        public BackupProgressInfo(
            BackupState? backupState = default(BackupState?),
            DateTime? timeStampUtc = default(DateTime?),
            Guid? backupId = default(Guid?),
            string backupLocation = default(string),
            Epoch epochOfLastBackupRecord = default(Epoch),
            string lsnOfLastBackupRecord = default(string),
            FabricErrorError failureError = default(FabricErrorError))
        {
            this.BackupState = backupState;
            this.TimeStampUtc = timeStampUtc;
            this.BackupId = backupId;
            this.BackupLocation = backupLocation;
            this.EpochOfLastBackupRecord = epochOfLastBackupRecord;
            this.LsnOfLastBackupRecord = lsnOfLastBackupRecord;
            this.FailureError = failureError;
        }

        /// <summary>
        /// Gets represents the current state of the partition backup operation.
        /// . Possible values include: 'Invalid', 'Accepted', 'BackupInProgress', 'Success', 'Failure', 'Timeout'
        /// </summary>
        public BackupState? BackupState { get; }

        /// <summary>
        /// Gets timeStamp in UTC when operation succeeded or failed.
        /// </summary>
        public DateTime? TimeStampUtc { get; }

        /// <summary>
        /// Gets unique ID of the newly created backup.
        /// </summary>
        public Guid? BackupId { get; }

        /// <summary>
        /// Gets location, relative to the backup store, of the newly created backup.
        /// </summary>
        public string BackupLocation { get; }

        /// <summary>
        /// Gets specifies the epoch of the last record included in backup.
        /// </summary>
        public Epoch EpochOfLastBackupRecord { get; }

        /// <summary>
        /// Gets the LSN of last record included in backup.
        /// </summary>
        public string LsnOfLastBackupRecord { get; }

        /// <summary>
        /// Gets denotes the failure encountered in performing backup operation.
        /// </summary>
        public FabricErrorError FailureError { get; }
    }
}
