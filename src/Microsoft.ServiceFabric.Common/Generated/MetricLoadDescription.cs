// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specifies metric load information.
    /// </summary>
    public partial class MetricLoadDescription
    {
        /// <summary>
        /// Initializes a new instance of the MetricLoadDescription class.
        /// </summary>
        /// <param name="metricName">The name of the reported metric.</param>
        /// <param name="currentLoad">The current value of the metric load.</param>
        /// <param name="predictedLoad">The predicted value of the metric load. Predicted metric load values is currently a
        /// preview feature. It allows predicted load values to be reported and used at the Service Fabric side, but that
        /// feature is currently not enabled.</param>
        public MetricLoadDescription(
            string metricName = default(string),
            long? currentLoad = default(long?),
            long? predictedLoad = default(long?))
        {
            this.MetricName = metricName;
            this.CurrentLoad = currentLoad;
            this.PredictedLoad = predictedLoad;
        }

        /// <summary>
        /// Gets the name of the reported metric.
        /// </summary>
        public string MetricName { get; }

        /// <summary>
        /// Gets the current value of the metric load.
        /// </summary>
        public long? CurrentLoad { get; }

        /// <summary>
        /// Gets the predicted value of the metric load. Predicted metric load values is currently a preview feature. It allows
        /// predicted load values to be reported and used at the Service Fabric side, but that feature is currently not
        /// enabled.
        /// </summary>
        public long? PredictedLoad { get; }
    }
}
