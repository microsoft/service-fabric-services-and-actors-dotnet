// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes a volume provided by an Azure Files file share.
    /// </summary>
    public partial class VolumeProviderParametersAzureFile
    {
        /// <summary>
        /// Initializes a new instance of the VolumeProviderParametersAzureFile class.
        /// </summary>
        /// <param name="accountName">Name of the Azure storage account for the File Share.</param>
        /// <param name="shareName">Name of the Azure Files file share that provides storage for the volume.</param>
        /// <param name="accountKey">Access key of the Azure storage account for the File Share.</param>
        public VolumeProviderParametersAzureFile(
            string accountName,
            string shareName,
            string accountKey = default(string))
        {
            accountName.ThrowIfNull(nameof(accountName));
            shareName.ThrowIfNull(nameof(shareName));
            this.AccountName = accountName;
            this.ShareName = shareName;
            this.AccountKey = accountKey;
        }

        /// <summary>
        /// Gets name of the Azure storage account for the File Share.
        /// </summary>
        public string AccountName { get; }

        /// <summary>
        /// Gets access key of the Azure storage account for the File Share.
        /// </summary>
        public string AccountKey { get; }

        /// <summary>
        /// Gets name of the Azure Files file share that provides storage for the volume.
        /// </summary>
        public string ShareName { get; }
    }
}
