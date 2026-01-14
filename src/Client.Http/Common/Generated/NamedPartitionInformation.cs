// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the partition information for the name as a string that is based on partition schemes.
    /// </summary>
    public partial class NamedPartitionInformation : PartitionInformation
    {
        /// <summary>
        /// Initializes a new instance of the NamedPartitionInformation class.
        /// </summary>
        /// <param name="id">An internal ID used by Service Fabric to uniquely identify a partition. This is a randomly
        /// generated GUID when the service was created. The partition ID is unique and does not change for the lifetime of the
        /// service. If the same service was deleted and recreated the IDs of its partitions would be different.</param>
        /// <param name="name">Name of the partition.</param>
        public NamedPartitionInformation(
            PartitionId id = default(PartitionId),
            string name = default(string))
            : base(
                Common.ServicePartitionKind.Named,
                id)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets name of the partition.
        /// </summary>
        public string Name { get; }
    }
}
