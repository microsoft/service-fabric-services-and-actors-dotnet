// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a backup point which can be used to trigger a restore.
    /// </summary>
    public partial class BackupInfo
    {
        /// <summary>
        /// Initializes a new instance of the BackupInfo class.
        /// </summary>
        /// <param name="backupId">Unique backup ID .</param>
        /// <param name="backupChainId">Unique backup chain ID. All backups part of the same chain has the same backup chain
        /// id. A backup chain is comprised of 1 full backup and multiple incremental backups.</param>
        /// <param name="applicationName">Name of the Service Fabric application this partition backup belongs to.</param>
        /// <param name="serviceName">Name of the Service Fabric service this partition backup belongs to.</param>
        /// <param name="partitionInformation">Information about the partition to which this backup belongs to</param>
        /// <param name="backupLocation">Location of the backup, relative to the backup store.</param>
        /// <param name="backupType">Describes the type of backup, whether its full or incremental.
        /// . Possible values include: 'Invalid', 'Full', 'Incremental'</param>
        /// <param name="epochOfLastBackupRecord">Epoch of the last record in this backup.</param>
        /// <param name="lsnOfLastBackupRecord">LSN of the last record in this backup.</param>
        /// <param name="creationTimeUtc">The date time when this backup was taken.</param>
        /// <param name="serviceManifestVersion">Manifest Version of the service this partition backup belongs to.</param>
        /// <param name="failureError">Denotes the failure encountered in getting backup point information.</param>
        public BackupInfo(
            Guid? backupId = default(Guid?),
            Guid? backupChainId = default(Guid?),
            string applicationName = default(string),
            string serviceName = default(string),
            PartitionInformation partitionInformation = default(PartitionInformation),
            string backupLocation = default(string),
            BackupType? backupType = default(BackupType?),
            Epoch epochOfLastBackupRecord = default(Epoch),
            string lsnOfLastBackupRecord = default(string),
            DateTime? creationTimeUtc = default(DateTime?),
            string serviceManifestVersion = default(string),
            FabricErrorError failureError = default(FabricErrorError))
        {
            this.BackupId = backupId;
            this.BackupChainId = backupChainId;
            this.ApplicationName = applicationName;
            this.ServiceName = serviceName;
            this.PartitionInformation = partitionInformation;
            this.BackupLocation = backupLocation;
            this.BackupType = backupType;
            this.EpochOfLastBackupRecord = epochOfLastBackupRecord;
            this.LsnOfLastBackupRecord = lsnOfLastBackupRecord;
            this.CreationTimeUtc = creationTimeUtc;
            this.ServiceManifestVersion = serviceManifestVersion;
            this.FailureError = failureError;
        }

        /// <summary>
        /// Gets unique backup ID .
        /// </summary>
        public Guid? BackupId { get; }

        /// <summary>
        /// Gets unique backup chain ID. All backups part of the same chain has the same backup chain id. A backup chain is
        /// comprised of 1 full backup and multiple incremental backups.
        /// </summary>
        public Guid? BackupChainId { get; }

        /// <summary>
        /// Gets name of the Service Fabric application this partition backup belongs to.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets name of the Service Fabric service this partition backup belongs to.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets information about the partition to which this backup belongs to
        /// </summary>
        public PartitionInformation PartitionInformation { get; }

        /// <summary>
        /// Gets location of the backup, relative to the backup store.
        /// </summary>
        public string BackupLocation { get; }

        /// <summary>
        /// Gets the type of backup, whether its full or incremental.
        /// . Possible values include: 'Invalid', 'Full', 'Incremental'
        /// </summary>
        public BackupType? BackupType { get; }

        /// <summary>
        /// Gets epoch of the last record in this backup.
        /// </summary>
        public Epoch EpochOfLastBackupRecord { get; }

        /// <summary>
        /// Gets LSN of the last record in this backup.
        /// </summary>
        public string LsnOfLastBackupRecord { get; }

        /// <summary>
        /// Gets the date time when this backup was taken.
        /// </summary>
        public DateTime? CreationTimeUtc { get; }

        /// <summary>
        /// Gets manifest Version of the service this partition backup belongs to.
        /// </summary>
        public string ServiceManifestVersion { get; }

        /// <summary>
        /// Gets denotes the failure encountered in getting backup point information.
        /// </summary>
        public FabricErrorError FailureError { get; }
    }
}
