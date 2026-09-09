// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KeyValueStoreProviderKind.
    /// </summary>
    public enum KeyValueStoreProviderKind
    {
        /// <summary>
        /// The provider kind has not been determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// Extensible Storage Engine. Copy details are available via ProviderCopyDetail.
        /// </summary>
        Ese,

        /// <summary>
        /// Transactional store provider. Copy details are not currently populated for TStore-backed replicas.
        /// </summary>
        TStore,
    }
}
