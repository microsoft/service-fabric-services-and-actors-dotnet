// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Safety check that ensures that a quorum of replicas are not lost for a partition.
    /// </summary>
    public partial class EnsurePartitionQuorumSafetyCheck : PartitionSafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the EnsurePartitionQuorumSafetyCheck class.
        /// </summary>
        /// <param name="partitionId">Id of the partition which is undergoing the safety check.</param>
        public EnsurePartitionQuorumSafetyCheck(
            PartitionId partitionId = default(PartitionId))
            : base(
                Common.SafetyCheckKind.EnsurePartitionQuorum,
                partitionId)
        {
        }
    }
}
