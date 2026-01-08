// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for BackupStorageKind.
    /// </summary>
    public enum BackupStorageKind
    {
        /// <summary>
        /// Indicates an invalid backup storage kind. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates file/ SMB share to be used as backup storage.
        /// </summary>
        FileShare,

        /// <summary>
        /// Indicates Azure blob store to be used as backup storage.
        /// </summary>
        AzureBlobStore,

        /// <summary>
        /// Indicates Dsms Azure blob store to be used as backup storage.
        /// </summary>
        DsmsAzureBlobStore,

        /// <summary>
        /// Indicates Azure blob store to be used as backup storage using managed identity.
        /// </summary>
        ManagedIdentityAzureBlobStore,
    }
}
