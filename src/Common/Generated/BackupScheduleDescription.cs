// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the backup schedule parameters.
    /// </summary>
    public abstract partial class BackupScheduleDescription
    {
        /// <summary>
        /// Initializes a new instance of the BackupScheduleDescription class.
        /// </summary>
        /// <param name="scheduleKind">The kind of backup schedule, time based or frequency based.
        /// </param>
        protected BackupScheduleDescription(
            BackupScheduleKind? scheduleKind)
        {
            scheduleKind.ThrowIfNull(nameof(scheduleKind));
            this.ScheduleKind = scheduleKind;
        }

        /// <summary>
        /// Gets the kind of backup schedule, time based or frequency based.
        /// </summary>
        public BackupScheduleKind? ScheduleKind { get; }
    }
}
