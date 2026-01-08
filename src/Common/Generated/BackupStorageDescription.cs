// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for the backup storage.
    /// </summary>
    public abstract partial class BackupStorageDescription
    {
        /// <summary>
        /// Initializes a new instance of the BackupStorageDescription class.
        /// </summary>
        /// <param name="storageKind">The kind of backup storage, where backups are saved.
        /// </param>
        /// <param name="friendlyName">Friendly name for this backup storage.</param>
        protected BackupStorageDescription(
            BackupStorageKind? storageKind,
            string friendlyName = default(string))
        {
            storageKind.ThrowIfNull(nameof(storageKind));
            this.StorageKind = storageKind;
            this.FriendlyName = friendlyName;
        }

        /// <summary>
        /// Gets friendly name for this backup storage.
        /// </summary>
        public string FriendlyName { get; }

        /// <summary>
        /// Gets the kind of backup storage, where backups are saved.
        /// </summary>
        public BackupStorageKind? StorageKind { get; }
    }
}
