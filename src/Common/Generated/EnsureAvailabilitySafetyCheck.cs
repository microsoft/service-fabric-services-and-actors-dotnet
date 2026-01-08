// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Safety check that waits to ensure the availability of the partition. It waits until there are replicas available
    /// such that bringing down this replica will not cause availability loss for the partition.
    /// </summary>
    public partial class EnsureAvailabilitySafetyCheck : PartitionSafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the EnsureAvailabilitySafetyCheck class.
        /// </summary>
        /// <param name="partitionId">Id of the partition which is undergoing the safety check.</param>
        public EnsureAvailabilitySafetyCheck(
            PartitionId partitionId = default(PartitionId))
            : base(
                Common.SafetyCheckKind.EnsureAvailability,
                partitionId)
        {
        }
    }
}
