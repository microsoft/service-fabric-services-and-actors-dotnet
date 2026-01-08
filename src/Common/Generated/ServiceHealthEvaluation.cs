// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation for a service, containing information about the data and the algorithm used by health
    /// store to evaluate health. The evaluation is returned only when the aggregated health state is either Error or
    /// Warning.
    /// </summary>
    public partial class ServiceHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the ServiceHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="serviceName">Name of the service whose health evaluation is described by this object.</param>
        /// <param name="unhealthyEvaluations">List of unhealthy evaluations that led to the current aggregated health state of
        /// the service. The types of the unhealthy evaluations can be PartitionsHealthEvaluation or
        /// EventHealthEvaluation.</param>
        public ServiceHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            ServiceName serviceName = default(ServiceName),
            IEnumerable<HealthEvaluationWrapper> unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>))
            : base(
                Common.HealthEvaluationKind.Service,
                aggregatedHealthState,
                description)
        {
            this.ServiceName = serviceName;
            this.UnhealthyEvaluations = unhealthyEvaluations;
        }

        /// <summary>
        /// Gets name of the service whose health evaluation is described by this object.
        /// </summary>
        public ServiceName ServiceName { get; }

        /// <summary>
        /// Gets list of unhealthy evaluations that led to the current aggregated health state of the service. The types of the
        /// unhealthy evaluations can be PartitionsHealthEvaluation or EventHealthEvaluation.
        /// </summary>
        public IEnumerable<HealthEvaluationWrapper> UnhealthyEvaluations { get; }
    }
}
