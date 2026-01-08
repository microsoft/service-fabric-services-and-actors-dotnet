// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the days of the week that a Chaos Schedule Job will run for.
    /// </summary>
    public partial class ChaosScheduleJobActiveDaysOfWeek
    {
        /// <summary>
        /// Initializes a new instance of the ChaosScheduleJobActiveDaysOfWeek class.
        /// </summary>
        /// <param name="sunday">Indicates if the Chaos Schedule Job will run on Sunday</param>
        /// <param name="monday">Indicates if the Chaos Schedule Job will run on Monday</param>
        /// <param name="tuesday">Indicates if the Chaos Schedule Job will run on Tuesday</param>
        /// <param name="wednesday">Indicates if the Chaos Schedule Job will run on Wednesday</param>
        /// <param name="thursday">Indicates if the Chaos Schedule Job will run on Thursday</param>
        /// <param name="friday">Indicates if the Chaos Schedule Job will run on Friday</param>
        /// <param name="saturday">Indicates if the Chaos Schedule Job will run on Saturday</param>
        public ChaosScheduleJobActiveDaysOfWeek(
            bool? sunday = false,
            bool? monday = false,
            bool? tuesday = false,
            bool? wednesday = false,
            bool? thursday = false,
            bool? friday = false,
            bool? saturday = false)
        {
            this.Sunday = sunday;
            this.Monday = monday;
            this.Tuesday = tuesday;
            this.Wednesday = wednesday;
            this.Thursday = thursday;
            this.Friday = friday;
            this.Saturday = saturday;
        }

        /// <summary>
        /// Gets indicates if the Chaos Schedule Job will run on Sunday
        /// </summary>
        public bool? Sunday { get; }

        /// <summary>
        /// Gets indicates if the Chaos Schedule Job will run on Monday
        /// </summary>
        public bool? Monday { get; }

        /// <summary>
        /// Gets indicates if the Chaos Schedule Job will run on Tuesday
        /// </summary>
        public bool? Tuesday { get; }

        /// <summary>
        /// Gets indicates if the Chaos Schedule Job will run on Wednesday
        /// </summary>
        public bool? Wednesday { get; }

        /// <summary>
        /// Gets indicates if the Chaos Schedule Job will run on Thursday
        /// </summary>
        public bool? Thursday { get; }

        /// <summary>
        /// Gets indicates if the Chaos Schedule Job will run on Friday
        /// </summary>
        public bool? Friday { get; }

        /// <summary>
        /// Gets indicates if the Chaos Schedule Job will run on Saturday
        /// </summary>
        public bool? Saturday { get; }
    }
}
