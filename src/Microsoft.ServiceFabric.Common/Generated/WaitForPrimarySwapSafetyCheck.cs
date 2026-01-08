// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Safety check that waits for the primary replica to be moved out of the node before starting an upgrade to ensure
    /// the availability of the primary replica for the partition.
    /// </summary>
    public partial class WaitForPrimarySwapSafetyCheck : PartitionSafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the WaitForPrimarySwapSafetyCheck class.
        /// </summary>
        /// <param name="partitionId">Id of the partition which is undergoing the safety check.</param>
        public WaitForPrimarySwapSafetyCheck(
            PartitionId partitionId = default(PartitionId))
            : base(
                Common.SafetyCheckKind.WaitForPrimarySwap,
                partitionId)
        {
        }
    }
}
