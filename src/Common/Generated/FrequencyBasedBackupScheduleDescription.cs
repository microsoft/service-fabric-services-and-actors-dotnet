// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the frequency based backup schedule.
    /// </summary>
    public partial class FrequencyBasedBackupScheduleDescription : BackupScheduleDescription
    {
        /// <summary>
        /// Initializes a new instance of the FrequencyBasedBackupScheduleDescription class.
        /// </summary>
        /// <param name="interval">Defines the interval with which backups are periodically taken. It should be specified in
        /// ISO8601 format. Timespan in seconds is not supported and will be ignored while creating the policy.</param>
        public FrequencyBasedBackupScheduleDescription(
            TimeSpan? interval)
            : base(
                Common.BackupScheduleKind.FrequencyBased)
        {
            interval.ThrowIfNull(nameof(interval));
            this.Interval = interval;
        }

        /// <summary>
        /// Gets defines the interval with which backups are periodically taken. It should be specified in ISO8601 format.
        /// Timespan in seconds is not supported and will be ignored while creating the policy.
        /// </summary>
        public TimeSpan? Interval { get; }
    }
}
