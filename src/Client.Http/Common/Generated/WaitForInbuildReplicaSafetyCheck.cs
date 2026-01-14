// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Safety check that waits for the replica build operation to finish. This indicates that there is a replica that is
    /// going through the copy or is providing data for building another replica. Bring the node down will abort this copy
    /// operation which are typically expensive involving data movements.
    /// </summary>
    public partial class WaitForInbuildReplicaSafetyCheck : PartitionSafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the WaitForInbuildReplicaSafetyCheck class.
        /// </summary>
        /// <param name="partitionId">Id of the partition which is undergoing the safety check.</param>
        public WaitForInbuildReplicaSafetyCheck(
            PartitionId partitionId = default(PartitionId))
            : base(
                Common.SafetyCheckKind.WaitForInbuildReplica,
                partitionId)
        {
        }
    }
}
