// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a scaling policy related to an average load of a metric/resource of a service.
    /// </summary>
    public partial class AverageServiceLoadScalingTrigger : ScalingTriggerDescription
    {
        /// <summary>
        /// Initializes a new instance of the AverageServiceLoadScalingTrigger class.
        /// </summary>
        /// <param name="metricName">The name of the metric for which usage should be tracked.</param>
        /// <param name="lowerLoadThreshold">The lower limit of the load below which a scale in operation should be
        /// performed.</param>
        /// <param name="upperLoadThreshold">The upper limit of the load beyond which a scale out operation should be
        /// performed.</param>
        /// <param name="scaleIntervalInSeconds">The period in seconds on which a decision is made whether to scale or
        /// not.</param>
        /// <param name="useOnlyPrimaryLoad">Flag determines whether only the load of primary replica should be considered for
        /// scaling.
        /// If set to true, then trigger will only consider the load of primary replicas of stateful service.
        /// If set to false, trigger will consider load of all replicas.
        /// This parameter cannot be set to true for stateless service.
        /// </param>
        public AverageServiceLoadScalingTrigger(
            string metricName,
            string lowerLoadThreshold,
            string upperLoadThreshold,
            long? scaleIntervalInSeconds,
            bool? useOnlyPrimaryLoad)
            : base(
                Common.ScalingTriggerKind.AverageServiceLoad)
        {
            metricName.ThrowIfNull(nameof(metricName));
            lowerLoadThreshold.ThrowIfNull(nameof(lowerLoadThreshold));
            upperLoadThreshold.ThrowIfNull(nameof(upperLoadThreshold));
            scaleIntervalInSeconds.ThrowIfNull(nameof(scaleIntervalInSeconds));
            useOnlyPrimaryLoad.ThrowIfNull(nameof(useOnlyPrimaryLoad));
            scaleIntervalInSeconds?.ThrowIfOutOfInclusiveRange("scaleIntervalInSeconds", 0, 4294967295);
            this.MetricName = metricName;
            this.LowerLoadThreshold = lowerLoadThreshold;
            this.UpperLoadThreshold = upperLoadThreshold;
            this.ScaleIntervalInSeconds = scaleIntervalInSeconds;
            this.UseOnlyPrimaryLoad = useOnlyPrimaryLoad;
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

        /// <summary>
        /// Gets flag determines whether only the load of primary replica should be considered for scaling.
        /// If set to true, then trigger will only consider the load of primary replicas of stateful service.
        /// If set to false, trigger will consider load of all replicas.
        /// This parameter cannot be set to true for stateless service.
        /// </summary>
        public bool? UseOnlyPrimaryLoad { get; }
    }
}
