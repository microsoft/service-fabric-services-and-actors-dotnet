// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health evaluation of a HealthEvent that was reported on the entity.
    /// The health evaluation is returned when evaluating health of an entity results in Error or Warning.
    /// </summary>
    public partial class EventHealthEvaluation : HealthEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the EventHealthEvaluation class.
        /// </summary>
        /// <param name="aggregatedHealthState">The health state of a Service Fabric entity such as Cluster, Node, Application,
        /// Service, Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="description">Description of the health evaluation, which represents a summary of the evaluation
        /// process.</param>
        /// <param name="considerWarningAsError">Indicates whether warnings are treated with the same severity as errors. The
        /// field is specified in the health policy used to evaluate the entity.</param>
        /// <param name="unhealthyEvent">Represents health information reported on a health entity, such as cluster,
        /// application or node, with additional metadata added by the Health Manager.
        /// </param>
        public EventHealthEvaluation(
            HealthState? aggregatedHealthState = default(HealthState?),
            string description = default(string),
            bool? considerWarningAsError = default(bool?),
            HealthEvent unhealthyEvent = default(HealthEvent))
            : base(
                Common.HealthEvaluationKind.Event,
                aggregatedHealthState,
                description)
        {
            this.ConsiderWarningAsError = considerWarningAsError;
            this.UnhealthyEvent = unhealthyEvent;
        }

        /// <summary>
        /// Gets indicates whether warnings are treated with the same severity as errors. The field is specified in the health
        /// policy used to evaluate the entity.
        /// </summary>
        public bool? ConsiderWarningAsError { get; }

        /// <summary>
        /// Gets represents health information reported on a health entity, such as cluster, application or node, with
        /// additional metadata added by the Health Manager.
        /// </summary>
        public HealthEvent UnhealthyEvent { get; }
    }
}
