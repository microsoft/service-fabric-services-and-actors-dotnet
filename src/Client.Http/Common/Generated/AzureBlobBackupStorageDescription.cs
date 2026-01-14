// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for Azure blob store used for storing and enumerating backups.
    /// </summary>
    public partial class AzureBlobBackupStorageDescription : BackupStorageDescription
    {
        /// <summary>
        /// Initializes a new instance of the AzureBlobBackupStorageDescription class.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to the Azure blob store.</param>
        /// <param name="containerName">The name of the container in the blob store to store and enumerate backups
        /// from.</param>
        /// <param name="friendlyName">Friendly name for this backup storage.</param>
        public AzureBlobBackupStorageDescription(
            string connectionString,
            string containerName,
            string friendlyName = default(string))
            : base(
                Common.BackupStorageKind.AzureBlobStore,
                friendlyName)
        {
            connectionString.ThrowIfNull(nameof(connectionString));
            containerName.ThrowIfNull(nameof(containerName));
            this.ConnectionString = connectionString;
            this.ContainerName = containerName;
        }

        /// <summary>
        /// Gets the connection string to connect to the Azure blob store.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets the name of the container in the blob store to store and enumerate backups from.
        /// </summary>
        public string ContainerName { get; }
    }
}
