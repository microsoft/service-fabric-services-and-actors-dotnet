// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A record of the times when the repair task entered each state.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class RepairTaskHistory
    {
        /// <summary>
        /// Initializes a new instance of the RepairTaskHistory class.
        /// </summary>
        /// <param name="createdUtcTimestamp">The time when the repair task entered the Created state.</param>
        /// <param name="claimedUtcTimestamp">The time when the repair task entered the Claimed state.</param>
        /// <param name="preparingUtcTimestamp">The time when the repair task entered the Preparing state.</param>
        /// <param name="approvedUtcTimestamp">The time when the repair task entered the Approved state</param>
        /// <param name="executingUtcTimestamp">The time when the repair task entered the Executing state</param>
        /// <param name="restoringUtcTimestamp">The time when the repair task entered the Restoring state</param>
        /// <param name="completedUtcTimestamp">The time when the repair task entered the Completed state</param>
        /// <param name="preparingHealthCheckStartUtcTimestamp">The time when the repair task started the health check in the
        /// Preparing state.</param>
        /// <param name="preparingHealthCheckEndUtcTimestamp">The time when the repair task completed the health check in the
        /// Preparing state.</param>
        /// <param name="restoringHealthCheckStartUtcTimestamp">The time when the repair task started the health check in the
        /// Restoring state.</param>
        /// <param name="restoringHealthCheckEndUtcTimestamp">The time when the repair task completed the health check in the
        /// Restoring state.</param>
        public RepairTaskHistory(
            DateTime? createdUtcTimestamp = default(DateTime?),
            DateTime? claimedUtcTimestamp = default(DateTime?),
            DateTime? preparingUtcTimestamp = default(DateTime?),
            DateTime? approvedUtcTimestamp = default(DateTime?),
            DateTime? executingUtcTimestamp = default(DateTime?),
            DateTime? restoringUtcTimestamp = default(DateTime?),
            DateTime? completedUtcTimestamp = default(DateTime?),
            DateTime? preparingHealthCheckStartUtcTimestamp = default(DateTime?),
            DateTime? preparingHealthCheckEndUtcTimestamp = default(DateTime?),
            DateTime? restoringHealthCheckStartUtcTimestamp = default(DateTime?),
            DateTime? restoringHealthCheckEndUtcTimestamp = default(DateTime?))
        {
            this.CreatedUtcTimestamp = createdUtcTimestamp;
            this.ClaimedUtcTimestamp = claimedUtcTimestamp;
            this.PreparingUtcTimestamp = preparingUtcTimestamp;
            this.ApprovedUtcTimestamp = approvedUtcTimestamp;
            this.ExecutingUtcTimestamp = executingUtcTimestamp;
            this.RestoringUtcTimestamp = restoringUtcTimestamp;
            this.CompletedUtcTimestamp = completedUtcTimestamp;
            this.PreparingHealthCheckStartUtcTimestamp = preparingHealthCheckStartUtcTimestamp;
            this.PreparingHealthCheckEndUtcTimestamp = preparingHealthCheckEndUtcTimestamp;
            this.RestoringHealthCheckStartUtcTimestamp = restoringHealthCheckStartUtcTimestamp;
            this.RestoringHealthCheckEndUtcTimestamp = restoringHealthCheckEndUtcTimestamp;
        }

        /// <summary>
        /// Gets the time when the repair task entered the Created state.
        /// </summary>
        public DateTime? CreatedUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task entered the Claimed state.
        /// </summary>
        public DateTime? ClaimedUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task entered the Preparing state.
        /// </summary>
        public DateTime? PreparingUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task entered the Approved state
        /// </summary>
        public DateTime? ApprovedUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task entered the Executing state
        /// </summary>
        public DateTime? ExecutingUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task entered the Restoring state
        /// </summary>
        public DateTime? RestoringUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task entered the Completed state
        /// </summary>
        public DateTime? CompletedUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task started the health check in the Preparing state.
        /// </summary>
        public DateTime? PreparingHealthCheckStartUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task completed the health check in the Preparing state.
        /// </summary>
        public DateTime? PreparingHealthCheckEndUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task started the health check in the Restoring state.
        /// </summary>
        public DateTime? RestoringHealthCheckStartUtcTimestamp { get; }

        /// <summary>
        /// Gets the time when the repair task completed the health check in the Restoring state.
        /// </summary>
        public DateTime? RestoringHealthCheckEndUtcTimestamp { get; }
    }
}
