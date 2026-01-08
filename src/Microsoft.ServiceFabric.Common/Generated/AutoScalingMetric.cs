// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the metric that is used for triggering auto scaling operation. Derived classes will describe resources or
    /// metrics.
    /// </summary>
    public abstract partial class AutoScalingMetric
    {
        /// <summary>
        /// Initializes a new instance of the AutoScalingMetric class.
        /// </summary>
        /// <param name="kind">Enumerates the metrics that are used for triggering auto scaling.</param>
        protected AutoScalingMetric(
            AutoScalingMetricKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets enumerates the metrics that are used for triggering auto scaling.
        /// </summary>
        public AutoScalingMetricKind? Kind { get; }
    }
}
