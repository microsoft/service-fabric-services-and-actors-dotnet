// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the resource that is used for triggering auto scaling.
    /// </summary>
    public partial class AutoScalingResourceMetric : AutoScalingMetric
    {
        /// <summary>
        /// Initializes a new instance of the AutoScalingResourceMetric class.
        /// </summary>
        /// <param name="name">Name of the resource. Possible values include: 'cpu', 'memoryInGB'
        /// 
        /// Enumerates the resources that are used for triggering auto scaling.
        /// </param>
        public AutoScalingResourceMetric(
            AutoScalingResourceMetricName? name)
            : base(
                Common.AutoScalingMetricKind.Resource)
        {
            name.ThrowIfNull(nameof(name));
            this.Name = name;
        }

        /// <summary>
        /// Gets name of the resource. Possible values include: 'cpu', 'memoryInGB'
        /// 
        /// Enumerates the resources that are used for triggering auto scaling.
        /// </summary>
        public AutoScalingResourceMetricName? Name { get; }
    }
}
