// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KeyValueStoreReplicaCopyMode.
    /// </summary>
    public enum KeyValueStoreReplicaCopyMode
    {
        /// <summary>
        /// The copy mode has not been determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// The primary streams the ESE database file directly to the secondary. Only used for full copies.
        /// </summary>
        Physical,

        /// <summary>
        /// The primary transfers state to the secondary row-by-row. Required when store formats are incompatible or when
        /// performing a partial copy.
        /// </summary>
        Logical,
    }
}
