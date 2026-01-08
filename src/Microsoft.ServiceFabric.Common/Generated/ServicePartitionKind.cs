// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServicePartitionKind.
    /// </summary>
    public enum ServicePartitionKind
    {
        /// <summary>
        /// Indicates the partition kind is invalid. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that there is only one partition, and SingletonPartitionSchemeDescription was specified while creating
        /// the service. The value is 1.
        /// </summary>
        Singleton,

        /// <summary>
        /// Indicates that the partition is based on Int64 key ranges, and UniformInt64RangePartitionSchemeDescription was
        /// specified while creating the service. The value is 2.
        /// </summary>
        Int64Range,

        /// <summary>
        /// Indicates that the partition is based on string names, and NamedPartitionInformation  was specified while creating
        /// the service. The value is 3.
        /// </summary>
        Named,
    }
}
