// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Parameters for forcing completion of a stuck self-reconfiguration on a partition.
    /// Use this when a self-reconfiguring service has a pending reconfiguration that
    /// is not making forward progress. Only activation-state reconfigurations (e.g.,
    /// a replica coming up or going down) can be completed; role-transition
    /// reconfigurations (e.g., Primary/Secondary swap) are not supported.
    /// The RequestSequenceNumber and ReportId values must match.
    /// </summary>
    public partial class CompleteSelfReconfigurationDescription
    {
        /// <summary>
        /// Initializes a new instance of the CompleteSelfReconfigurationDescription class.
        /// </summary>
        /// <param name="partitionId">The partition ID of the service with the stuck reconfiguration.</param>
        /// <param name="requestSequenceNumber">The sequence number from the reconfiguration request. This value must match
        /// ReportId.</param>
        /// <param name="requestGenerationNumber">The generation number from the reconfiguration request. Identifies which
        /// generation of the request to complete.</param>
        /// <param name="reportId">The reconfiguration report sequence number. This value must match
        /// RequestSequenceNumber.</param>
        public CompleteSelfReconfigurationDescription(
            Guid? partitionId,
            long? requestSequenceNumber,
            long? requestGenerationNumber,
            long? reportId)
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            requestSequenceNumber.ThrowIfNull(nameof(requestSequenceNumber));
            requestGenerationNumber.ThrowIfNull(nameof(requestGenerationNumber));
            reportId.ThrowIfNull(nameof(reportId));
            this.PartitionId = partitionId;
            this.RequestSequenceNumber = requestSequenceNumber;
            this.RequestGenerationNumber = requestGenerationNumber;
            this.ReportId = reportId;
        }

        /// <summary>
        /// Gets the partition ID of the service with the stuck reconfiguration.
        /// </summary>
        public Guid? PartitionId { get; }

        /// <summary>
        /// Gets the sequence number from the reconfiguration request. This value must match ReportId.
        /// </summary>
        public long? RequestSequenceNumber { get; }

        /// <summary>
        /// Gets the generation number from the reconfiguration request. Identifies which generation of the request to
        /// complete.
        /// </summary>
        public long? RequestGenerationNumber { get; }

        /// <summary>
        /// Gets the reconfiguration report sequence number. This value must match RequestSequenceNumber.
        /// </summary>
        public long? ReportId { get; }
    }
}
