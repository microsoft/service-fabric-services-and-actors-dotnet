// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a safety check for the service partition being performed by service fabric before continuing with
    /// operations.
    /// </summary>
    public partial class PartitionSafetyCheck : SafetyCheck
    {
        /// <summary>
        /// Initializes a new instance of the PartitionSafetyCheck class.
        /// </summary>
        /// <param name="kind">The kind of safety check performed by service fabric before continuing with the operations.
        /// These checks ensure the availability of the service and the reliability of the state. Following are the kinds of
        /// safety checks.</param>
        /// <param name="partitionId">Id of the partition which is undergoing the safety check.</param>
        public PartitionSafetyCheck(
            SafetyCheckKind? kind,
            PartitionId partitionId = default(PartitionId))
            : base(
                kind)
        {
            this.PartitionId = partitionId;
        }

        /// <summary>
        /// Gets id of the partition which is undergoing the safety check.
        /// </summary>
        public PartitionId PartitionId { get; }
    }
}
