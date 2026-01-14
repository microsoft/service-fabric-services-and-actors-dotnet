// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a repetition rule and parameters of Chaos to be used with the Chaos Schedule.
    /// </summary>
    public partial class ChaosScheduleJob
    {
        /// <summary>
        /// Initializes a new instance of the ChaosScheduleJob class.
        /// </summary>
        /// <param name="chaosParameters">A reference to which Chaos Parameters of the Chaos Schedule to use.
        /// </param>
        /// <param name="days">Defines the days of the week that a Chaos Schedule Job will run for.</param>
        /// <param name="times">A list of Time Ranges that specify when during active days that this job will run. The times
        /// are interpreted as UTC.
        /// </param>
        public ChaosScheduleJob(
            string chaosParameters = default(string),
            ChaosScheduleJobActiveDaysOfWeek days = default(ChaosScheduleJobActiveDaysOfWeek),
            IEnumerable<TimeRange> times = default(IEnumerable<TimeRange>))
        {
            this.ChaosParameters = chaosParameters;
            this.Days = days;
            this.Times = times;
        }

        /// <summary>
        /// Gets a reference to which Chaos Parameters of the Chaos Schedule to use.
        /// </summary>
        public string ChaosParameters { get; }

        /// <summary>
        /// Gets defines the days of the week that a Chaos Schedule Job will run for.
        /// </summary>
        public ChaosScheduleJobActiveDaysOfWeek Days { get; }

        /// <summary>
        /// Gets a list of Time Ranges that specify when during active days that this job will run. The times are interpreted
        /// as UTC.
        /// </summary>
        public IEnumerable<TimeRange> Times { get; }
    }
}
