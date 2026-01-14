// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about load reported by replica.
    /// </summary>
    public partial class LoadMetricReportInfo
    {
        /// <summary>
        /// Initializes a new instance of the LoadMetricReportInfo class.
        /// </summary>
        /// <param name="name">The name of the metric.</param>
        /// <param name="value">The value of the load for the metric. In future releases of Service Fabric this parameter will
        /// be deprecated in favor of CurrentValue.</param>
        /// <param name="currentValue">The double value of the load for the metric.</param>
        /// <param name="lastReportedUtc">The UTC time when the load is reported.</param>
        public LoadMetricReportInfo(
            string name = default(string),
            int? value = default(int?),
            string currentValue = default(string),
            DateTime? lastReportedUtc = default(DateTime?))
        {
            this.Name = name;
            this.Value = value;
            this.CurrentValue = currentValue;
            this.LastReportedUtc = lastReportedUtc;
        }

        /// <summary>
        /// Gets the name of the metric.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the load for the metric. In future releases of Service Fabric this parameter will be deprecated
        /// in favor of CurrentValue.
        /// </summary>
        public int? Value { get; }

        /// <summary>
        /// Gets the double value of the load for the metric.
        /// </summary>
        public string CurrentValue { get; }

        /// <summary>
        /// Gets the UTC time when the load is reported.
        /// </summary>
        public DateTime? LastReportedUtc { get; }
    }
}
