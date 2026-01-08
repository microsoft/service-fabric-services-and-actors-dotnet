// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the time based backup schedule.
    /// </summary>
    public partial class TimeBasedBackupScheduleDescription : BackupScheduleDescription
    {
        /// <summary>
        /// Initializes a new instance of the TimeBasedBackupScheduleDescription class.
        /// </summary>
        /// <param name="scheduleFrequencyType">Describes the frequency with which to run the time based backup schedule.
        /// . Possible values include: 'Invalid', 'Daily', 'Weekly'</param>
        /// <param name="runTimes">Represents the list of exact time during the day in ISO8601 format. Like '19:00:00' will
        /// represent '7PM' during the day. Date specified along with time will be ignored.</param>
        /// <param name="runDays">List of days of a week when to trigger the periodic backup. This is valid only when the
        /// backup schedule frequency type is weekly.</param>
        public TimeBasedBackupScheduleDescription(
            BackupScheduleFrequencyType? scheduleFrequencyType,
            IEnumerable<DateTime?> runTimes,
            IEnumerable<DayOfWeek?> runDays = default(IEnumerable<DayOfWeek?>))
            : base(
                Common.BackupScheduleKind.TimeBased)
        {
            scheduleFrequencyType.ThrowIfNull(nameof(scheduleFrequencyType));
            runTimes.ThrowIfNull(nameof(runTimes));
            this.ScheduleFrequencyType = scheduleFrequencyType;
            this.RunTimes = runTimes;
            this.RunDays = runDays;
        }

        /// <summary>
        /// Gets the frequency with which to run the time based backup schedule.
        /// . Possible values include: 'Invalid', 'Daily', 'Weekly'
        /// </summary>
        public BackupScheduleFrequencyType? ScheduleFrequencyType { get; }

        /// <summary>
        /// Gets list of days of a week when to trigger the periodic backup. This is valid only when the backup schedule
        /// frequency type is weekly.
        /// </summary>
        public IEnumerable<DayOfWeek?> RunDays { get; }

        /// <summary>
        /// Gets represents the list of exact time during the day in ISO8601 format. Like '19:00:00' will represent '7PM'
        /// during the day. Date specified along with time will be ignored.
        /// </summary>
        public IEnumerable<DateTime?> RunTimes { get; }
    }
}
