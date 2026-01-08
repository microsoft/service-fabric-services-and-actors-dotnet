// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specifies metric loads of a partition's specific secondary replica or instance.
    /// </summary>
    public partial class ReplicaMetricLoadDescription
    {
        /// <summary>
        /// Initializes a new instance of the ReplicaMetricLoadDescription class.
        /// </summary>
        /// <param name="nodeName">Node name of a specific secondary replica or instance.</param>
        /// <param name="replicaOrInstanceLoadEntries">Loads of a different metrics for a partition's secondary replica or
        /// instance.</param>
        public ReplicaMetricLoadDescription(
            string nodeName = default(string),
            IEnumerable<MetricLoadDescription> replicaOrInstanceLoadEntries = default(IEnumerable<MetricLoadDescription>))
        {
            this.NodeName = nodeName;
            this.ReplicaOrInstanceLoadEntries = replicaOrInstanceLoadEntries;
        }

        /// <summary>
        /// Gets node name of a specific secondary replica or instance.
        /// </summary>
        public string NodeName { get; }

        /// <summary>
        /// Gets loads of a different metrics for a partition's secondary replica or instance.
        /// </summary>
        public IEnumerable<MetricLoadDescription> ReplicaOrInstanceLoadEntries { get; }
    }
}
