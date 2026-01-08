// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a partition that is singleton. The services with singleton partitioning scheme are effectively
    /// non-partitioned. They only have one partition.
    /// </summary>
    public partial class SingletonPartitionInformation : PartitionInformation
    {
        /// <summary>
        /// Initializes a new instance of the SingletonPartitionInformation class.
        /// </summary>
        /// <param name="id">An internal ID used by Service Fabric to uniquely identify a partition. This is a randomly
        /// generated GUID when the service was created. The partition ID is unique and does not change for the lifetime of the
        /// service. If the same service was deleted and recreated the IDs of its partitions would be different.</param>
        public SingletonPartitionInformation(
            PartitionId id = default(PartitionId))
            : base(
                Common.ServicePartitionKind.Singleton,
                id)
        {
        }
    }
}
