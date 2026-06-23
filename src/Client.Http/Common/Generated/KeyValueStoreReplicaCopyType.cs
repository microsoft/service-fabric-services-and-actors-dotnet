// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KeyValueStoreReplicaCopyType.
    /// </summary>
    public enum KeyValueStoreReplicaCopyType
    {
        /// <summary>
        /// The copy type has not been determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// All state data is copied from primary to secondary.
        /// </summary>
        Full,

        /// <summary>
        /// Only incremental changes since the secondary's last known sequence number are copied.
        /// </summary>
        Partial,
    }
}
