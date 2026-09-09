// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Copy details for a key value store replica build.
    /// ProviderCopyDetail is a polymorphic projection of the IDL CopyDetails pointer.
    /// </summary>
    public partial class InbuildKeyValueStoreCopyDetail : InbuildReplicaCopyDetail
    {
        /// <summary>
        /// Initializes a new instance of the InbuildKeyValueStoreCopyDetail class.
        /// </summary>
        /// <param name="providerCopyDetail">Provider-specific copy metadata. The Kind discriminator within this object
        /// determines the concrete type. Present only when the state provider populates copy
        /// details; null otherwise.
        /// </param>
        public InbuildKeyValueStoreCopyDetail(
            KeyValueStoreProviderCopyDetail providerCopyDetail = default(KeyValueStoreProviderCopyDetail))
            : base(
                Common.ReplicaKind.KeyValueStore)
        {
            this.ProviderCopyDetail = providerCopyDetail;
        }

        /// <summary>
        /// Gets provider-specific copy metadata. The Kind discriminator within this object
        /// determines the concrete type. Present only when the state provider populates copy
        /// details; null otherwise.
        /// </summary>
        public KeyValueStoreProviderCopyDetail ProviderCopyDetail { get; }
    }
}
