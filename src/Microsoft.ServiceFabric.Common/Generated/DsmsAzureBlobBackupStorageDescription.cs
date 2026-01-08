// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for Dsms Azure blob store used for storing and enumerating backups.
    /// </summary>
    public partial class DsmsAzureBlobBackupStorageDescription : BackupStorageDescription
    {
        /// <summary>
        /// Initializes a new instance of the DsmsAzureBlobBackupStorageDescription class.
        /// </summary>
        /// <param name="storageCredentialsSourceLocation">The source location of the storage credentials to connect to the
        /// Dsms Azure blob store.</param>
        /// <param name="containerName">The name of the container in the blob store to store and enumerate backups
        /// from.</param>
        /// <param name="friendlyName">Friendly name for this backup storage.</param>
        public DsmsAzureBlobBackupStorageDescription(
            string storageCredentialsSourceLocation,
            string containerName,
            string friendlyName = default(string))
            : base(
                Common.BackupStorageKind.DsmsAzureBlobStore,
                friendlyName)
        {
            storageCredentialsSourceLocation.ThrowIfNull(nameof(storageCredentialsSourceLocation));
            containerName.ThrowIfNull(nameof(containerName));
            this.StorageCredentialsSourceLocation = storageCredentialsSourceLocation;
            this.ContainerName = containerName;
        }

        /// <summary>
        /// Gets the source location of the storage credentials to connect to the Dsms Azure blob store.
        /// </summary>
        public string StorageCredentialsSourceLocation { get; }

        /// <summary>
        /// Gets the name of the container in the blob store to store and enumerate backups from.
        /// </summary>
        public string ContainerName { get; }
    }
}
