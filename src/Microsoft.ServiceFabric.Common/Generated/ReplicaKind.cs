// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ReplicaKind.
    /// </summary>
    public enum ReplicaKind
    {
        /// <summary>
        /// Represents an invalid replica kind. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Represents a key value store replica. The value is 1.
        /// </summary>
        KeyValueStore,
    }
}
