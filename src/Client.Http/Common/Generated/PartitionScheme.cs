// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for PartitionScheme.
    /// </summary>
    public enum PartitionScheme
    {
        /// <summary>
        /// Indicates the partition kind is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the partition is based on string names, and is a SingletonPartitionSchemeDescription object, The
        /// value is 1.
        /// </summary>
        Singleton,

        /// <summary>
        /// Indicates that the partition is based on Int64 key ranges, and is a UniformInt64RangePartitionSchemeDescription
        /// object. The value is 2.
        /// </summary>
        UniformInt64Range,

        /// <summary>
        /// Indicates that the partition is based on string names, and is a NamedPartitionSchemeDescription object. The value
        /// is 3.
        /// </summary>
        Named,
    }
}
