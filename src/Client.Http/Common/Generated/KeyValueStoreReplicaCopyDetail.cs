// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Copy details for a key value store replica build. Contains a single ProviderCopyDetail
    /// property that holds provider-specific copy metadata, polymorphic by ProviderKind.
    /// Currently only ESE-backed replicas populate provider copy details.
    /// </summary>
    public partial class KeyValueStoreReplicaCopyDetail : InbuildReplicaCopyDetail
    {
        /// <summary>
        /// Initializes a new instance of the KeyValueStoreReplicaCopyDetail class.
        /// </summary>
        /// <param name="providerCopyDetail">Provider-specific copy metadata. The ProviderKind discriminator within this object
        /// determines the concrete type. Present only when the state provider populates copy
        /// details; null otherwise.
        /// </param>
        public KeyValueStoreReplicaCopyDetail(
            KeyValueStoreProviderCopyDetail providerCopyDetail = default(KeyValueStoreProviderCopyDetail))
            : base(
                Common.ReplicaKind.KeyValueStore)
        {
            this.ProviderCopyDetail = providerCopyDetail;
        }

        /// <summary>
        /// Gets provider-specific copy metadata. The ProviderKind discriminator within this object
        /// determines the concrete type. Present only when the state provider populates copy
        /// details; null otherwise.
        /// </summary>
        public KeyValueStoreProviderCopyDetail ProviderCopyDetail { get; }
    }
}
