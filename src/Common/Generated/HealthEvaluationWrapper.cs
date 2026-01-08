// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper object for health evaluation.
    /// </summary>
    public partial class HealthEvaluationWrapper
    {
        /// <summary>
        /// Initializes a new instance of the HealthEvaluationWrapper class.
        /// </summary>
        /// <param name="healthEvaluation">Represents a health evaluation which describes the data and the algorithm used by
        /// health manager to evaluate the health of an entity.</param>
        public HealthEvaluationWrapper(
            HealthEvaluation healthEvaluation = default(HealthEvaluation))
        {
            this.HealthEvaluation = healthEvaluation;
        }

        /// <summary>
        /// Gets represents a health evaluation which describes the data and the algorithm used by health manager to evaluate
        /// the health of an entity.
        /// </summary>
        public HealthEvaluation HealthEvaluation { get; }
    }
}
