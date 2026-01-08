// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Safety check that waits for the current reconfiguration of the partition to be completed before starting an
    /// upgrade.
    /// </summary>
    public partial class WaitForReconfigurationSafetyCheck : PartitionSafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the WaitForReconfigurationSafetyCheck class.
        /// </summary>
        /// <param name="partitionId">Id of the partition which is undergoing the safety check.</param>
        public WaitForReconfigurationSafetyCheck(
            PartitionId partitionId = default(PartitionId))
            : base(
                Common.SafetyCheckKind.WaitForReconfiguration,
                partitionId)
        {
        }
    }
}
