// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KvsReplicaCopyMode.
    /// </summary>
    public enum KvsReplicaCopyMode
    {
        /// <summary>
        /// The copy mode has not been determined. The value is 0.
        /// </summary>
        Unknown,

        /// <summary>
        /// The primary streams the ESE database file directly to the secondary. Only used for full copies. The value is 1.
        /// </summary>
        Physical,

        /// <summary>
        /// The primary transfers state to the secondary row-by-row. Required when store formats are incompatible or when
        /// performing a partial copy. The value is 2.
        /// </summary>
        Logical,
    }
}
