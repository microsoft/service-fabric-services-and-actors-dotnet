// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a health evaluation which describes the data and the algorithm used by health manager to evaluate the
    /// health of an entity.
    /// </summary>
    public abstract partial class HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the HealthEvaluation class.
        /// </summary>
        /// <param name="kind">The health manager in the cluster performs health evaluations in determining the aggregated
        /// health state of an entity. This enumeration provides information on the kind of evaluation that was performed.
        /// Following are the possible values.</param>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        protected HealthEvaluation(
            HealthEvaluationKind? kind,
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string))
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
            this.AggregatedHealthState = aggregatedHealthState;
            this.Description = description;
        }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? AggregatedHealthState { get; }

        /// <summary>
        /// Gets description of the health evaluation, which represents a summary of the evaluation process.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the health manager in the cluster performs health evaluations in determining the aggregated health state of an
        /// entity. This enumeration provides information on the kind of evaluation that was performed. Following are the
        /// possible values.
        /// </summary>
        public HealthEvaluationKind? Kind { get; }
    }
}
