// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains a description of Chaos.
    /// </summary>
    public partial class Chaos
    {
        /// <summary>
        /// Initializes a new instance of the Chaos class.
        /// </summary>
        /// <param name="chaosParameters">If Chaos is running, these are the parameters Chaos is running with.</param>
        /// <param name="status">Current status of the Chaos run.
        /// . Possible values include: 'Invalid', 'Running', 'Stopped'</param>
        /// <param name="scheduleStatus">Current status of the schedule.
        /// . Possible values include: 'Invalid', 'Stopped', 'Active', 'Expired', 'Pending'</param>
        public Chaos(
            ChaosParameters chaosParameters = default(ChaosParameters),
            ChaosStatus? status = default(ChaosStatus?),
            ChaosScheduleStatus? scheduleStatus = default(ChaosScheduleStatus?))
        {
            this.ChaosParameters = chaosParameters;
            this.Status = status;
            this.ScheduleStatus = scheduleStatus;
        }

        /// <summary>
        /// Gets if Chaos is running, these are the parameters Chaos is running with.
        /// </summary>
        public ChaosParameters ChaosParameters { get; }

        /// <summary>
        /// Gets current status of the Chaos run.
        /// . Possible values include: 'Invalid', 'Running', 'Stopped'
        /// </summary>
        public ChaosStatus? Status { get; }

        /// <summary>
        /// Gets current status of the schedule.
        /// . Possible values include: 'Invalid', 'Stopped', 'Active', 'Expired', 'Pending'
        /// </summary>
        public ChaosScheduleStatus? ScheduleStatus { get; }
    }
}
