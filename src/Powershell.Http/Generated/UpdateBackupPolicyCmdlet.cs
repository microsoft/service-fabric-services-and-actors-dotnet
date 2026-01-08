// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Updates the backup policy.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFBackupPolicy")]
    public partial class UpdateBackupPolicyCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets FrequencyBased flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_FrequencyBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_FrequencyBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        public SwitchParameter FrequencyBased { get; set; }

        /// <summary>
        /// Gets or sets TimeBased flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public SwitchParameter TimeBased { get; set; }

        /// <summary>
        /// Gets or sets AzureBlobStore flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_FrequencyBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        public SwitchParameter AzureBlobStore { get; set; }

        /// <summary>
        /// Gets or sets FileShare flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        public SwitchParameter FileShare { get; set; }

        /// <summary>
        /// Gets or sets DsmsAzureBlobStore flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_FrequencyBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        public SwitchParameter DsmsAzureBlobStore { get; set; }

        /// <summary>
        /// Gets or sets ManagedIdentityAzureBlobStore flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public SwitchParameter ManagedIdentityAzureBlobStore { get; set; }

        /// <summary>
        /// Gets or sets Basic flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "_FrequencyBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "_FrequencyBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public SwitchParameter Basic { get; set; }

        /// <summary>
        /// Gets or sets Name. The unique name identifying this backup policy.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets AutoRestoreOnDataLoss. Specifies whether to trigger restore automatically using the latest available
        /// backup in case the partition experiences a data loss event.
        /// </summary>
        [Parameter(Mandatory = true, Position = 4)]
        public bool? AutoRestoreOnDataLoss { get; set; }

        /// <summary>
        /// Gets or sets MaxIncrementalBackups. Defines the maximum number of incremental backups to be taken between two full
        /// backups. This is just the upper limit. A full backup may be taken before specified number of incremental backups
        /// are completed in one of the following conditions
        /// - The replica has never taken a full backup since it has become primary,
        /// - Some of the log records since the last backup has been truncated, or
        /// - Replica passed the MaxAccumulatedBackupLogSizeInMB limit.
        /// </summary>
        [Parameter(Mandatory = true, Position = 5)]
        public int? MaxIncrementalBackups { get; set; }

        /// <summary>
        /// Gets or sets Interval. Defines the interval with which backups are periodically taken. It should be specified in
        /// ISO8601 format. Timespan in seconds is not supported and will be ignored while creating the policy.
        /// </summary>
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_FrequencyBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_FrequencyBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        public TimeSpan? Interval { get; set; }

        /// <summary>
        /// Gets or sets ScheduleFrequencyType. Describes the frequency with which to run the time based backup schedule.
        /// . Possible values include: 'Invalid', 'Daily', 'Weekly'
        /// </summary>
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public BackupScheduleFrequencyType? ScheduleFrequencyType { get; set; }

        /// <summary>
        /// Gets or sets RunTimes. Represents the list of exact time during the day in ISO8601 format. Like '19:00:00' will
        /// represent '7PM' during the day. Date specified along with time will be ignored.
        /// </summary>
        [Parameter(Mandatory = true, Position = 8, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 8, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        [Parameter(Mandatory = true, Position = 8, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 8, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public IEnumerable<DateTime?> RunTimes { get; set; }

        /// <summary>
        /// Gets or sets ConnectionString. The connection string to connect to the Azure blob store.
        /// </summary>
        [Parameter(Mandatory = true, Position = 9, ParameterSetName = "_FrequencyBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 9, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets ContainerName. The name of the container in the blob store to store and enumerate backups from.
        /// </summary>
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_FrequencyBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_FrequencyBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 10, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public string ContainerName { get; set; }

        /// <summary>
        /// Gets or sets Path. UNC path of the file share where to store or enumerate backups from.
        /// </summary>
        [Parameter(Mandatory = true, Position = 11, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = true, Position = 11, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets StorageCredentialsSourceLocation. The source location of the storage credentials to connect to the
        /// Dsms Azure blob store.
        /// </summary>
        [Parameter(Mandatory = true, Position = 12, ParameterSetName = "_FrequencyBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 12, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        public string StorageCredentialsSourceLocation { get; set; }

        /// <summary>
        /// Gets or sets ManagedIdentityType. The type of managed identity to be used to connect to Azure Blob Store via
        /// Managed Identity.
        /// . Possible values include: 'Invalid', 'VMSS', 'Cluster'
        /// </summary>
        [Parameter(Mandatory = true, Position = 13, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 13, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public ManagedIdentityType? ManagedIdentityType { get; set; }

        /// <summary>
        /// Gets or sets BlobServiceUri. The Blob Service Uri to connect to the Azure blob store..
        /// </summary>
        [Parameter(Mandatory = true, Position = 14, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 14, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public string BlobServiceUri { get; set; }

        /// <summary>
        /// Gets or sets RetentionDuration. It is the minimum duration for which a backup created, will remain stored in the
        /// storage and might get deleted after that span of time. It should be specified in ISO8601 format.
        /// </summary>
        [Parameter(Mandatory = true, Position = 15, ParameterSetName = "_FrequencyBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 15, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = true, Position = 15, ParameterSetName = "_FrequencyBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 15, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 15, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 15, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        [Parameter(Mandatory = true, Position = 15, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = true, Position = 15, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public TimeSpan? RetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets BackupPolicyName. The name of the backup policy.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 16)]
        public string BackupPolicyName { get; set; }

        /// <summary>
        /// Gets or sets RunDays. List of days of a week when to trigger the periodic backup. This is valid only when the
        /// backup schedule frequency type is weekly.
        /// </summary>
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public IEnumerable<DayOfWeek?> RunDays { get; set; }

        /// <summary>
        /// Gets or sets FriendlyName. Friendly name for this backup storage.
        /// </summary>
        [Parameter(Mandatory = false, Position = 18)]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets PrimaryUserName. Primary user name to access the file share.
        /// </summary>
        [Parameter(Mandatory = false, Position = 19, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 19, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        public string PrimaryUserName { get; set; }

        /// <summary>
        /// Gets or sets PrimaryPassword. Primary password to access the share location.
        /// </summary>
        [Parameter(Mandatory = false, Position = 20, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 20, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        public string PrimaryPassword { get; set; }

        /// <summary>
        /// Gets or sets SecondaryUserName. Secondary user name to access the file share.
        /// </summary>
        [Parameter(Mandatory = false, Position = 21, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 21, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        public string SecondaryUserName { get; set; }

        /// <summary>
        /// Gets or sets SecondaryPassword. Secondary password to access the share location
        /// </summary>
        [Parameter(Mandatory = false, Position = 22, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 22, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        public string SecondaryPassword { get; set; }

        /// <summary>
        /// Gets or sets ManagedIdentityClientId. The ClientId of User-Assigned Managed Identity
        /// </summary>
        [Parameter(Mandatory = false, Position = 23, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 23, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public Guid? ManagedIdentityClientId { get; set; }

        /// <summary>
        /// Gets or sets MinimumNumberOfBackups. It is the minimum number of backups to be retained at any point of time. If
        /// specified with a non zero value, backups will not be deleted even if the backups have gone past retention duration
        /// and have number of backups less than or equal to it.
        /// </summary>
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_FrequencyBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_FrequencyBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_FrequencyBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_FrequencyBased__ManagedIdentityAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_TimeBased__AzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_TimeBased__FileShare__Basic_")]
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_TimeBased__DsmsAzureBlobStore__Basic_")]
        [Parameter(Mandatory = false, Position = 24, ParameterSetName = "_TimeBased__ManagedIdentityAzureBlobStore__Basic_")]
        public int? MinimumNumberOfBackups { get; set; }

        /// <summary>
        /// Gets or sets CompressionType. Specifies the compression type to be used for backups. Default is CLUSTER_DEFINED.
        /// Possible values include: 'CLUSTER_DEFINED', 'ZIP', 'ZSTANDARD'
        /// </summary>
        [Parameter(Mandatory = false, Position = 25)]
        public CompressionType? CompressionType { get; set; }

        /// <summary>
        /// Gets or sets QuickRecovery. Specifies the quick recovery strategy for this backup policy. Default is Disabled.
        /// Possible values include: 'Disabled', 'FromPrimary'
        /// </summary>
        [Parameter(Mandatory = false, Position = 26)]
        public QuickRecovery? QuickRecovery { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 27)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets ValidateConnection. Specifies whether to validate the storage connection and credentials before
        /// creating or updating the backup policies.
        /// </summary>
        [Parameter(Mandatory = false, Position = 28)]
        public bool? ValidateConnection { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            BackupScheduleDescription backupScheduleDescription = null;
            if (this.FrequencyBased.IsPresent)
            {
                backupScheduleDescription = new FrequencyBasedBackupScheduleDescription(
                    interval: this.Interval);
            }
            else if (this.TimeBased.IsPresent)
            {
                backupScheduleDescription = new TimeBasedBackupScheduleDescription(
                    scheduleFrequencyType: this.ScheduleFrequencyType,
                    runTimes: this.RunTimes,
                    runDays: this.RunDays);
            }

            BackupStorageDescription backupStorageDescription = null;
            if (this.AzureBlobStore.IsPresent)
            {
                backupStorageDescription = new AzureBlobBackupStorageDescription(
                    connectionString: this.ConnectionString,
                    containerName: this.ContainerName,
                    friendlyName: this.FriendlyName);
            }
            else if (this.FileShare.IsPresent)
            {
                backupStorageDescription = new FileShareBackupStorageDescription(
                    path: this.Path,
                    friendlyName: this.FriendlyName,
                    primaryUserName: this.PrimaryUserName,
                    primaryPassword: this.PrimaryPassword,
                    secondaryUserName: this.SecondaryUserName,
                    secondaryPassword: this.SecondaryPassword);
            }
            else if (this.DsmsAzureBlobStore.IsPresent)
            {
                backupStorageDescription = new DsmsAzureBlobBackupStorageDescription(
                    storageCredentialsSourceLocation: this.StorageCredentialsSourceLocation,
                    containerName: this.ContainerName,
                    friendlyName: this.FriendlyName);
            }
            else if (this.ManagedIdentityAzureBlobStore.IsPresent)
            {
                backupStorageDescription = new ManagedIdentityAzureBlobBackupStorageDescription(
                    managedIdentityType: this.ManagedIdentityType,
                    blobServiceUri: this.BlobServiceUri,
                    containerName: this.ContainerName,
                    friendlyName: this.FriendlyName,
                    managedIdentityClientId: this.ManagedIdentityClientId);
            }

            RetentionPolicyDescription retentionPolicyDescription = null;
            if (this.Basic.IsPresent)
            {
                retentionPolicyDescription = new BasicRetentionPolicyDescription(
                    retentionDuration: this.RetentionDuration,
                    minimumNumberOfBackups: this.MinimumNumberOfBackups);
            }

            var backupPolicyDescription = new BackupPolicyDescription(
            name: this.Name,
            autoRestoreOnDataLoss: this.AutoRestoreOnDataLoss,
            maxIncrementalBackups: this.MaxIncrementalBackups,
            schedule: backupScheduleDescription,
            storage: backupStorageDescription,
            retentionPolicy: retentionPolicyDescription,
            compressionType: this.CompressionType,
            quickRecovery: this.QuickRecovery);

            this.ServiceFabricClient.BackupRestore.UpdateBackupPolicyAsync(
                backupPolicyDescription: backupPolicyDescription,
                backupPolicyName: this.BackupPolicyName,
                serverTimeout: this.ServerTimeout,
                validateConnection: this.ValidateConnection,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
