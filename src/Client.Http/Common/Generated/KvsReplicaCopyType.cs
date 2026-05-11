// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for KvsReplicaCopyType.
    /// </summary>
    public enum KvsReplicaCopyType
    {
        /// <summary>
        /// The copy type has not been determined. The value is 0.
        /// </summary>
        Unknown,

        /// <summary>
        /// All state data is copied from primary to secondary. The value is 1.
        /// </summary>
        Full,

        /// <summary>
        /// Only incremental changes since the secondary's last known sequence number are copied. The value is 2.
        /// </summary>
        Partial,
    }
}
