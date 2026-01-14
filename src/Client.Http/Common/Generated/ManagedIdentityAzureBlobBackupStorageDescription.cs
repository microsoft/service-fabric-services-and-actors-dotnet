// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for Azure blob store (connected using managed identity) used for storing and enumerating
    /// backups.
    /// </summary>
    public partial class ManagedIdentityAzureBlobBackupStorageDescription : BackupStorageDescription
    {
        /// <summary>
        /// Initializes a new instance of the ManagedIdentityAzureBlobBackupStorageDescription class.
        /// </summary>
        /// <param name="managedIdentityType">The type of managed identity to be used to connect to Azure Blob Store via
        /// Managed Identity.
        /// . Possible values include: 'Invalid', 'VMSS', 'Cluster'</param>
        /// <param name="blobServiceUri">The Blob Service Uri to connect to the Azure blob store..</param>
        /// <param name="containerName">The name of the container in the blob store to store and enumerate backups
        /// from.</param>
        /// <param name="friendlyName">Friendly name for this backup storage.</param>
        /// <param name="managedIdentityClientId">The ClientId of User-Assigned Managed Identity</param>
        public ManagedIdentityAzureBlobBackupStorageDescription(
            ManagedIdentityType? managedIdentityType,
            string blobServiceUri,
            string containerName,
            string friendlyName = default(string),
            Guid? managedIdentityClientId = default(Guid?))
            : base(
                Common.BackupStorageKind.ManagedIdentityAzureBlobStore,
                friendlyName)
        {
            managedIdentityType.ThrowIfNull(nameof(managedIdentityType));
            blobServiceUri.ThrowIfNull(nameof(blobServiceUri));
            containerName.ThrowIfNull(nameof(containerName));
            this.ManagedIdentityType = managedIdentityType;
            this.BlobServiceUri = blobServiceUri;
            this.ContainerName = containerName;
            this.ManagedIdentityClientId = managedIdentityClientId;
        }

        /// <summary>
        /// Gets the type of managed identity to be used to connect to Azure Blob Store via Managed Identity.
        /// . Possible values include: 'Invalid', 'VMSS', 'Cluster'
        /// </summary>
        public ManagedIdentityType? ManagedIdentityType { get; }

        /// <summary>
        /// Gets the Blob Service Uri to connect to the Azure blob store..
        /// </summary>
        public string BlobServiceUri { get; }

        /// <summary>
        /// Gets the name of the container in the blob store to store and enumerate backups from.
        /// </summary>
        public string ContainerName { get; }

        /// <summary>
        /// Gets the ClientId of User-Assigned Managed Identity
        /// </summary>
        public Guid? ManagedIdentityClientId { get; }
    }
}
