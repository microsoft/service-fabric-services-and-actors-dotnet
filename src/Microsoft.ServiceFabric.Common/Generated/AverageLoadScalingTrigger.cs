// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the average load trigger used for auto scaling.
    /// </summary>
    public partial class AverageLoadScalingTrigger : AutoScalingTrigger
    {
        /// <summary>
        /// Initializes a new instance of the AverageLoadScalingTrigger class.
        /// </summary>
        /// <param name="metric">Description of the metric that is used for scaling.</param>
        /// <param name="lowerLoadThreshold">Lower load threshold (if average load is below this threshold, service will scale
        /// down).</param>
        /// <param name="upperLoadThreshold">Upper load threshold (if average load is above this threshold, service will scale
        /// up).</param>
        /// <param name="scaleIntervalInSeconds">Scale interval that indicates how often will this trigger be checked.</param>
        public AverageLoadScalingTrigger(
            AutoScalingMetric metric,
            double? lowerLoadThreshold,
            double? upperLoadThreshold,
            int? scaleIntervalInSeconds)
            : base(
                Common.AutoScalingTriggerKind.AverageLoad)
        {
            metric.ThrowIfNull(nameof(metric));
            lowerLoadThreshold.ThrowIfNull(nameof(lowerLoadThreshold));
            upperLoadThreshold.ThrowIfNull(nameof(upperLoadThreshold));
            scaleIntervalInSeconds.ThrowIfNull(nameof(scaleIntervalInSeconds));
            scaleIntervalInSeconds?.ThrowIfLessThan("scaleIntervalInSeconds", 60);
            this.Metric = metric;
            this.LowerLoadThreshold = lowerLoadThreshold;
            this.UpperLoadThreshold = upperLoadThreshold;
            this.ScaleIntervalInSeconds = scaleIntervalInSeconds;
        }

        /// <summary>
        /// Gets description of the metric that is used for scaling.
        /// </summary>
        public AutoScalingMetric Metric { get; }

        /// <summary>
        /// Gets lower load threshold (if average load is below this threshold, service will scale down).
        /// </summary>
        public double? LowerLoadThreshold { get; }

        /// <summary>
        /// Gets upper load threshold (if average load is above this threshold, service will scale up).
        /// </summary>
        public double? UpperLoadThreshold { get; }

        /// <summary>
        /// Gets scale interval that indicates how often will this trigger be checked.
        /// </summary>
        public int? ScaleIntervalInSeconds { get; }
    }
}
