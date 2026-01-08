// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a scaling trigger related to an average load of a metric/resource of a partition.
    /// </summary>
    public partial class AveragePartitionLoadScalingTrigger : ScalingTriggerDescription
    {
        /// <summary>
        /// Initializes a new instance of the AveragePartitionLoadScalingTrigger class.
        /// </summary>
        /// <param name="metricName">The name of the metric for which usage should be tracked.</param>
        /// <param name="lowerLoadThreshold">The lower limit of the load below which a scale in operation should be
        /// performed.</param>
        /// <param name="upperLoadThreshold">The upper limit of the load beyond which a scale out operation should be
        /// performed.</param>
        /// <param name="scaleIntervalInSeconds">The period in seconds on which a decision is made whether to scale or
        /// not.</param>
        public AveragePartitionLoadScalingTrigger(
            string metricName,
            string lowerLoadThreshold,
            string upperLoadThreshold,
            long? scaleIntervalInSeconds)
            : base(
                Common.ScalingTriggerKind.AveragePartitionLoad)
        {
            metricName.ThrowIfNull(nameof(metricName));
            lowerLoadThreshold.ThrowIfNull(nameof(lowerLoadThreshold));
            upperLoadThreshold.ThrowIfNull(nameof(upperLoadThreshold));
            scaleIntervalInSeconds.ThrowIfNull(nameof(scaleIntervalInSeconds));
            scaleIntervalInSeconds?.ThrowIfOutOfInclusiveRange("scaleIntervalInSeconds", 0, 4294967295);
            this.MetricName = metricName;
            this.LowerLoadThreshold = lowerLoadThreshold;
            this.UpperLoadThreshold = upperLoadThreshold;
            this.ScaleIntervalInSeconds = scaleIntervalInSeconds;
        }

        /// <summary>
        /// Gets the name of the metric for which usage should be tracked.
        /// </summary>
        public string MetricName { get; }

        /// <summary>
        /// Gets the lower limit of the load below which a scale in operation should be performed.
        /// </summary>
        public string LowerLoadThreshold { get; }

        /// <summary>
        /// Gets the upper limit of the load beyond which a scale out operation should be performed.
        /// </summary>
        public string UpperLoadThreshold { get; }

        /// <summary>
        /// Gets the period in seconds on which a decision is made whether to scale or not.
        /// </summary>
        public long? ScaleIntervalInSeconds { get; }
    }
}
