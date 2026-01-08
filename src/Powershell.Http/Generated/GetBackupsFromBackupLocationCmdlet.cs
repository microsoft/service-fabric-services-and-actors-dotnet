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
    /// Gets the list of backups available for the specified backed up entity at the specified backup location.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFBackupsFromBackupLocation")]
    public partial class GetBackupsFromBackupLocationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets AzureBlobStore flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_AzureBlobStore__Application_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_AzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_AzureBlobStore__Partition_")]
        public SwitchParameter AzureBlobStore { get; set; }

        /// <summary>
        /// Gets or sets FileShare flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_FileShare__Application_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_FileShare__Service_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_FileShare__Partition_")]
        public SwitchParameter FileShare { get; set; }

        /// <summary>
        /// Gets or sets DsmsAzureBlobStore flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_DsmsAzureBlobStore__Application_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_DsmsAzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_DsmsAzureBlobStore__Partition_")]
        public SwitchParameter DsmsAzureBlobStore { get; set; }

        /// <summary>
        /// Gets or sets ManagedIdentityAzureBlobStore flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_ManagedIdentityAzureBlobStore__Application_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_ManagedIdentityAzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_ManagedIdentityAzureBlobStore__Partition_")]
        public SwitchParameter ManagedIdentityAzureBlobStore { get; set; }

        /// <summary>
        /// Gets or sets Application flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_AzureBlobStore__Application_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_FileShare__Application_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_DsmsAzureBlobStore__Application_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_ManagedIdentityAzureBlobStore__Application_")]
        public SwitchParameter Application { get; set; }

        /// <summary>
        /// Gets or sets Service flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_AzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_FileShare__Service_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_DsmsAzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_ManagedIdentityAzureBlobStore__Service_")]
        public SwitchParameter Service { get; set; }

        /// <summary>
        /// Gets or sets Partition flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_AzureBlobStore__Partition_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_FileShare__Partition_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_DsmsAzureBlobStore__Partition_")]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "_ManagedIdentityAzureBlobStore__Partition_")]
        public SwitchParameter Partition { get; set; }

        /// <summary>
        /// Gets or sets ConnectionString. The connection string to connect to the Azure blob store.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "_AzureBlobStore__Application_")]
        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "_AzureBlobStore__Service_")]
        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "_AzureBlobStore__Partition_")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets ContainerName. The name of the container in the blob store to store and enumerate backups from.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_AzureBlobStore__Application_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_AzureBlobStore__Service_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_AzureBlobStore__Partition_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_DsmsAzureBlobStore__Application_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_DsmsAzureBlobStore__Service_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_DsmsAzureBlobStore__Partition_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_ManagedIdentityAzureBlobStore__Application_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_ManagedIdentityAzureBlobStore__Service_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_ManagedIdentityAzureBlobStore__Partition_")]
        public string ContainerName { get; set; }

        /// <summary>
        /// Gets or sets Path. UNC path of the file share where to store or enumerate backups from.
        /// </summary>
        [Parameter(Mandatory = true, Position = 4, ParameterSetName = "_FileShare__Application_")]
        [Parameter(Mandatory = true, Position = 4, ParameterSetName = "_FileShare__Service_")]
        [Parameter(Mandatory = true, Position = 4, ParameterSetName = "_FileShare__Partition_")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets StorageCredentialsSourceLocation. The source location of the storage credentials to connect to the
        /// Dsms Azure blob store.
        /// </summary>
        [Parameter(Mandatory = true, Position = 5, ParameterSetName = "_DsmsAzureBlobStore__Application_")]
        [Parameter(Mandatory = true, Position = 5, ParameterSetName = "_DsmsAzureBlobStore__Service_")]
        [Parameter(Mandatory = true, Position = 5, ParameterSetName = "_DsmsAzureBlobStore__Partition_")]
        public string StorageCredentialsSourceLocation { get; set; }

        /// <summary>
        /// Gets or sets ManagedIdentityType. The type of managed identity to be used to connect to Azure Blob Store via
        /// Managed Identity.
        /// . Possible values include: 'Invalid', 'VMSS', 'Cluster'
        /// </summary>
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_ManagedIdentityAzureBlobStore__Application_")]
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_ManagedIdentityAzureBlobStore__Service_")]
        [Parameter(Mandatory = true, Position = 6, ParameterSetName = "_ManagedIdentityAzureBlobStore__Partition_")]
        public ManagedIdentityType? ManagedIdentityType { get; set; }

        /// <summary>
        /// Gets or sets BlobServiceUri. The Blob Service Uri to connect to the Azure blob store..
        /// </summary>
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_ManagedIdentityAzureBlobStore__Application_")]
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_ManagedIdentityAzureBlobStore__Service_")]
        [Parameter(Mandatory = true, Position = 7, ParameterSetName = "_ManagedIdentityAzureBlobStore__Partition_")]
        public string BlobServiceUri { get; set; }

        /// <summary>
        /// Gets or sets StartDateTimeFilter. Specifies the start date time in ISO8601 from which to enumerate backups. If not
        /// specified, backups are enumerated from the beginning.
        /// </summary>
        [Parameter(Mandatory = false, Position = 8)]
        public DateTime? StartDateTimeFilter { get; set; }

        /// <summary>
        /// Gets or sets EndDateTimeFilter. Specifies the end date time in ISO8601 till which to enumerate backups. If not
        /// specified, backups are enumerated till the end.
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public DateTime? EndDateTimeFilter { get; set; }

        /// <summary>
        /// Gets or sets Latest. If specified as true, gets the most recent backup (within the specified time range) for every
        /// partition under the specified backup entity.
        /// </summary>
        [Parameter(Mandatory = false, Position = 10)]
        public bool? Latest { get; set; } = false;

        /// <summary>
        /// Gets or sets FriendlyName. Friendly name for this backup storage.
        /// </summary>
        [Parameter(Mandatory = false, Position = 11)]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets PrimaryUserName. Primary user name to access the file share.
        /// </summary>
        [Parameter(Mandatory = false, Position = 12, ParameterSetName = "_FileShare__Application_")]
        [Parameter(Mandatory = false, Position = 12, ParameterSetName = "_FileShare__Service_")]
        [Parameter(Mandatory = false, Position = 12, ParameterSetName = "_FileShare__Partition_")]
        public string PrimaryUserName { get; set; }

        /// <summary>
        /// Gets or sets PrimaryPassword. Primary password to access the share location.
        /// </summary>
        [Parameter(Mandatory = false, Position = 13, ParameterSetName = "_FileShare__Application_")]
        [Parameter(Mandatory = false, Position = 13, ParameterSetName = "_FileShare__Service_")]
        [Parameter(Mandatory = false, Position = 13, ParameterSetName = "_FileShare__Partition_")]
        public string PrimaryPassword { get; set; }

        /// <summary>
        /// Gets or sets SecondaryUserName. Secondary user name to access the file share.
        /// </summary>
        [Parameter(Mandatory = false, Position = 14, ParameterSetName = "_FileShare__Application_")]
        [Parameter(Mandatory = false, Position = 14, ParameterSetName = "_FileShare__Service_")]
        [Parameter(Mandatory = false, Position = 14, ParameterSetName = "_FileShare__Partition_")]
        public string SecondaryUserName { get; set; }

        /// <summary>
        /// Gets or sets SecondaryPassword. Secondary password to access the share location
        /// </summary>
        [Parameter(Mandatory = false, Position = 15, ParameterSetName = "_FileShare__Application_")]
        [Parameter(Mandatory = false, Position = 15, ParameterSetName = "_FileShare__Service_")]
        [Parameter(Mandatory = false, Position = 15, ParameterSetName = "_FileShare__Partition_")]
        public string SecondaryPassword { get; set; }

        /// <summary>
        /// Gets or sets ManagedIdentityClientId. The ClientId of User-Assigned Managed Identity
        /// </summary>
        [Parameter(Mandatory = false, Position = 16, ParameterSetName = "_ManagedIdentityAzureBlobStore__Application_")]
        [Parameter(Mandatory = false, Position = 16, ParameterSetName = "_ManagedIdentityAzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 16, ParameterSetName = "_ManagedIdentityAzureBlobStore__Partition_")]
        public Guid? ManagedIdentityClientId { get; set; }

        /// <summary>
        /// Gets or sets ApplicationName. The name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_AzureBlobStore__Application_")]
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_FileShare__Application_")]
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_DsmsAzureBlobStore__Application_")]
        [Parameter(Mandatory = false, Position = 17, ParameterSetName = "_ManagedIdentityAzureBlobStore__Application_")]
        public ApplicationName ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets ServiceName. The full name of the service with 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_AzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_FileShare__Service_")]
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_DsmsAzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_ManagedIdentityAzureBlobStore__Service_")]
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_AzureBlobStore__Partition_")]
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_FileShare__Partition_")]
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_DsmsAzureBlobStore__Partition_")]
        [Parameter(Mandatory = false, Position = 18, ParameterSetName = "_ManagedIdentityAzureBlobStore__Partition_")]
        public ServiceName ServiceName { get; set; }

        /// <summary>
        /// Gets or sets PartitionId. The partition ID identifying the partition.
        /// </summary>
        [Parameter(Mandatory = false, Position = 19, ParameterSetName = "_AzureBlobStore__Partition_")]
        [Parameter(Mandatory = false, Position = 19, ParameterSetName = "_FileShare__Partition_")]
        [Parameter(Mandatory = false, Position = 19, ParameterSetName = "_DsmsAzureBlobStore__Partition_")]
        [Parameter(Mandatory = false, Position = 19, ParameterSetName = "_ManagedIdentityAzureBlobStore__Partition_")]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 20)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets MaxResults. The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.
        /// </summary>
        [Parameter(Mandatory = false, Position = 21)]
        public long? MaxResults { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
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

            BackupEntity backupEntity = null;
            if (this.Application.IsPresent)
            {
                backupEntity = new ApplicationBackupEntity(
                    applicationName: this.ApplicationName);
            }
            else if (this.Service.IsPresent)
            {
                backupEntity = new ServiceBackupEntity(
                    serviceName: this.ServiceName);
            }
            else if (this.Partition.IsPresent)
            {
                backupEntity = new PartitionBackupEntity(
                    serviceName: this.ServiceName,
                    partitionId: this.PartitionId);
            }

            var getBackupByStorageQueryDescription = new GetBackupByStorageQueryDescription(
            storage: backupStorageDescription,
            backupEntity: backupEntity,
            startDateTimeFilter: this.StartDateTimeFilter,
            endDateTimeFilter: this.EndDateTimeFilter,
            latest: this.Latest);

            var continuationToken = default(ContinuationToken);
            do
            {
                var result = this.ServiceFabricClient.BackupRestore.GetBackupsFromBackupLocationAsync(
                    getBackupByStorageQueryDescription: getBackupByStorageQueryDescription,
                    serverTimeout: this.ServerTimeout,
                    continuationToken: continuationToken,
                    maxResults: this.MaxResults,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result == null)
                {
                    break;
                }

                var count = 0;
                foreach (var item in result.Data)
                {
                    count++;
                    this.WriteObject(this.FormatOutput(item));
                }

                continuationToken = result.ContinuationToken;
                this.WriteDebug(string.Format(Resource.MsgCountAndContinuationToken, count, continuationToken));
            }
            while (continuationToken.Next);
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
