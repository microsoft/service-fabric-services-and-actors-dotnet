// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents information about how many health entities are in Ok, Warning and Error health state.
    /// </summary>
    public partial class HealthStateCount
    {
        /// <summary>
        /// Initializes a new instance of the HealthStateCount class.
        /// </summary>
        /// <param name="okCount">The number of health entities with aggregated health state Ok.</param>
        /// <param name="warningCount">The number of health entities with aggregated health state Warning.</param>
        /// <param name="errorCount">The number of health entities with aggregated health state Error.</param>
        public HealthStateCount(
            long? okCount = default(long?),
            long? warningCount = default(long?),
            long? errorCount = default(long?))
        {
            okCount?.ThrowIfLessThan("okCount", 0);
            warningCount?.ThrowIfLessThan("warningCount", 0);
            errorCount?.ThrowIfLessThan("errorCount", 0);
            this.OkCount = okCount;
            this.WarningCount = warningCount;
            this.ErrorCount = errorCount;
        }

        /// <summary>
        /// Gets the number of health entities with aggregated health state Ok.
        /// </summary>
        public long? OkCount { get; }

        /// <summary>
        /// Gets the number of health entities with aggregated health state Warning.
        /// </summary>
        public long? WarningCount { get; }

        /// <summary>
        /// Gets the number of health entities with aggregated health state Error.
        /// </summary>
        public long? ErrorCount { get; }
    }
}
