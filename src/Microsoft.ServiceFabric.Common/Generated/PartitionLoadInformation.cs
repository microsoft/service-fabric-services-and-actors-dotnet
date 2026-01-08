// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents load information for a partition, which contains the primary, secondary and auxiliary reported load
    /// metrics.
    /// In case there is no load reported, PartitionLoadInformation will contain the default load for the service of the
    /// partition.
    /// For default loads, LoadMetricReport's LastReportedUtc is set to 0.
    /// </summary>
    public partial class PartitionLoadInformation
    {
        /// <summary>
        /// Initializes a new instance of the PartitionLoadInformation class.
        /// </summary>
        /// <param name="partitionId">Id of the partition.</param>
        /// <param name="primaryLoadMetricReports">Array of load reports from the primary replica for this partition.</param>
        /// <param name="secondaryLoadMetricReports">Array of aggregated load reports from all secondary replicas for this
        /// partition.
        /// Array only contains the latest reported load for each metric.
        /// </param>
        /// <param name="auxiliaryLoadMetricReports">Array of aggregated load reports from all auxiliary replicas for this
        /// partition.
        /// Array only contains the latest reported load for each metric.
        /// </param>
        public PartitionLoadInformation(
            PartitionId partitionId = default(PartitionId),
            IEnumerable<LoadMetricReport> primaryLoadMetricReports = default(IEnumerable<LoadMetricReport>),
            IEnumerable<LoadMetricReport> secondaryLoadMetricReports = default(IEnumerable<LoadMetricReport>),
            IEnumerable<LoadMetricReport> auxiliaryLoadMetricReports = default(IEnumerable<LoadMetricReport>))
        {
            this.PartitionId = partitionId;
            this.PrimaryLoadMetricReports = primaryLoadMetricReports;
            this.SecondaryLoadMetricReports = secondaryLoadMetricReports;
            this.AuxiliaryLoadMetricReports = auxiliaryLoadMetricReports;
        }

        /// <summary>
        /// Gets id of the partition.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets array of load reports from the primary replica for this partition.
        /// </summary>
        public IEnumerable<LoadMetricReport> PrimaryLoadMetricReports { get; }

        /// <summary>
        /// Gets array of aggregated load reports from all secondary replicas for this partition.
        /// Array only contains the latest reported load for each metric.
        /// </summary>
        public IEnumerable<LoadMetricReport> SecondaryLoadMetricReports { get; }

        /// <summary>
        /// Gets array of aggregated load reports from all auxiliary replicas for this partition.
        /// Array only contains the latest reported load for each metric.
        /// </summary>
        public IEnumerable<LoadMetricReport> AuxiliaryLoadMetricReports { get; }
    }
}
