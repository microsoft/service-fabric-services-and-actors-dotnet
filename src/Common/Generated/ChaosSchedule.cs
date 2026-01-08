// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the schedule used by Chaos.
    /// </summary>
    public partial class ChaosSchedule
    {
        /// <summary>
        /// Initializes a new instance of the ChaosSchedule class.
        /// </summary>
        /// <param name="startDate">The date and time Chaos will start using this schedule.
        /// </param>
        /// <param name="expiryDate">The date and time Chaos will continue to use this schedule until.
        /// </param>
        /// <param name="chaosParametersDictionary">A mapping of string names to Chaos Parameters to be referenced by Chaos
        /// Schedule Jobs.
        /// </param>
        /// <param name="jobs">A list of all Chaos Schedule Jobs that will be automated by the schedule.
        /// </param>
        public ChaosSchedule(
            DateTime? startDate = default(DateTime?),
            DateTime? expiryDate = default(DateTime?),
            IEnumerable<ChaosParametersDictionaryItem> chaosParametersDictionary = default(IEnumerable<ChaosParametersDictionaryItem>),
            IEnumerable<ChaosScheduleJob> jobs = default(IEnumerable<ChaosScheduleJob>))
        {
            this.StartDate = startDate;
            this.ExpiryDate = expiryDate;
            this.ChaosParametersDictionary = chaosParametersDictionary;
            this.Jobs = jobs;
        }

        /// <summary>
        /// Gets the date and time Chaos will start using this schedule.
        /// </summary>
        public DateTime? StartDate { get; }

        /// <summary>
        /// Gets the date and time Chaos will continue to use this schedule until.
        /// </summary>
        public DateTime? ExpiryDate { get; }

        /// <summary>
        /// Gets a mapping of string names to Chaos Parameters to be referenced by Chaos Schedule Jobs.
        /// </summary>
        public IEnumerable<ChaosParametersDictionaryItem> ChaosParametersDictionary { get; }

        /// <summary>
        /// Gets a list of all Chaos Schedule Jobs that will be automated by the schedule.
        /// </summary>
        public IEnumerable<ChaosScheduleJob> Jobs { get; }
    }
}
