// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Safety check that waits for the primary replica that was moved out of the node due to upgrade to be placed back
    /// again on that node.
    /// </summary>
    public partial class WaitForPrimaryPlacementSafetyCheck : PartitionSafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the WaitForPrimaryPlacementSafetyCheck class.
        /// </summary>
        /// <param name="partitionId">Id of the partition which is undergoing the safety check.</param>
        public WaitForPrimaryPlacementSafetyCheck(
            PartitionId partitionId = default(PartitionId))
            : base(
                Common.SafetyCheckKind.WaitForPrimaryPlacement,
                partitionId)
        {
        }
    }
}
