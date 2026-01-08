// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the load metric report which contains the time metric was reported, its name and value.
    /// </summary>
    public partial class LoadMetricReport
    {
        /// <summary>
        /// Initializes a new instance of the LoadMetricReport class.
        /// </summary>
        /// <param name="lastReportedUtc">Gets the UTC time when the load was reported.</param>
        /// <param name="name">The name of the load metric.</param>
        /// <param name="value">The value of the load metric. In future releases of Service Fabric this parameter will be
        /// deprecated in favor of CurrentValue.</param>
        /// <param name="currentValue">The value of the load metric.</param>
        public LoadMetricReport(
            DateTime? lastReportedUtc = default(DateTime?),
            string name = default(string),
            string value = default(string),
            string currentValue = default(string))
        {
            this.LastReportedUtc = lastReportedUtc;
            this.Name = name;
            this.Value = value;
            this.CurrentValue = currentValue;
        }

        /// <summary>
        /// Gets the UTC time when the load was reported.
        /// </summary>
        public DateTime? LastReportedUtc { get; }

        /// <summary>
        /// Gets the name of the load metric.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the load metric. In future releases of Service Fabric this parameter will be deprecated in favor
        /// of CurrentValue.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the value of the load metric.
        /// </summary>
        public string CurrentValue { get; }
    }
}
