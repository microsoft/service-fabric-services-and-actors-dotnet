// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the Chaos Schedule used by Chaos and the version of the Chaos Schedule. The version value wraps back to 0
    /// after surpassing 2,147,483,647.
    /// </summary>
    public partial class ChaosScheduleDescription
    {
        /// <summary>
        /// Initializes a new instance of the ChaosScheduleDescription class.
        /// </summary>
        /// <param name="version">The version number of the Schedule.</param>
        /// <param name="schedule">Defines the schedule used by Chaos.</param>
        public ChaosScheduleDescription(
            int? version = default(int?),
            ChaosSchedule schedule = default(ChaosSchedule))
        {
            version?.ThrowIfLessThan("version", 0);
            this.Version = version;
            this.Schedule = schedule;
        }

        /// <summary>
        /// Gets the version number of the Schedule.
        /// </summary>
        public int? Version { get; }

        /// <summary>
        /// Gets defines the schedule used by Chaos.
        /// </summary>
        public ChaosSchedule Schedule { get; }
    }
}
