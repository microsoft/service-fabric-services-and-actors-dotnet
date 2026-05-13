// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base type for provider-specific copy details within a key value store replica build.
    /// The Kind property determines the storage provider (e.g., ESE or TStore) for this copy detail.
    /// Currently only ESE-backed replicas populate provider-specific copy fields.
    /// </summary>
    public abstract partial class KeyValueStoreProviderCopyDetail
    {
        /// <summary>
        /// Initializes a new instance of the KeyValueStoreProviderCopyDetail class.
        /// </summary>
        /// <param name="providerKind">The storage provider backing the key value store replica. Determines which copy detail
        /// type is populated during replica build.
        /// - Unknown - The provider kind has not been determined. The value is 0.
        /// - ESE - Extensible Storage Engine. Copy details are available via ProviderCopyDetail including
        /// copy type, mode, and format version information. The value is 1.
        /// - TStore - Transactional store provider. Copy details are not currently populated for
        /// TStore-backed replicas. The value is 2.
        /// </param>
        protected KeyValueStoreProviderCopyDetail(
            KeyValueStoreProviderKind? providerKind)
        {
            providerKind.ThrowIfNull(nameof(providerKind));
            this.ProviderKind = providerKind;
        }

        /// <summary>
        /// Gets the storage provider backing the key value store replica. Determines which copy detail
        /// type is populated during replica build.
        /// - Unknown - The provider kind has not been determined. The value is 0.
        /// - ESE - Extensible Storage Engine. Copy details are available via ProviderCopyDetail including
        /// copy type, mode, and format version information. The value is 1.
        /// - TStore - Transactional store provider. Copy details are not currently populated for
        /// TStore-backed replicas. The value is 2.
        /// </summary>
        public KeyValueStoreProviderKind? ProviderKind { get; }
    }
}
