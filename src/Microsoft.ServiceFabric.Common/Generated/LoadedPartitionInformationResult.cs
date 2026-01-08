// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents partition information.
    /// </summary>
    public partial class LoadedPartitionInformationResult
    {
        /// <summary>
        /// Initializes a new instance of the LoadedPartitionInformationResult class.
        /// </summary>
        /// <param name="serviceName">Name of the service this partition belongs to.</param>
        /// <param name="partitionId">Id of the partition.</param>
        /// <param name="metricName">Name of the metric for which this information is provided.</param>
        /// <param name="load">Load for metric.</param>
        public LoadedPartitionInformationResult(
            string serviceName,
            PartitionId partitionId,
            string metricName,
            long? load)
        {
            serviceName.ThrowIfNull(nameof(serviceName));
            partitionId.ThrowIfNull(nameof(partitionId));
            metricName.ThrowIfNull(nameof(metricName));
            load.ThrowIfNull(nameof(load));
            this.ServiceName = serviceName;
            this.PartitionId = partitionId;
            this.MetricName = metricName;
            this.Load = load;
        }

        /// <summary>
        /// Gets name of the service this partition belongs to.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets id of the partition.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets name of the metric for which this information is provided.
        /// </summary>
        public string MetricName { get; }

        /// <summary>
        /// Gets load for metric.
        /// </summary>
        public long? Load { get; }
    }
}
