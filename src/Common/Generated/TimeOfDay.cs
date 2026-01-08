// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an hour and minute of the day specified in 24 hour time.
    /// </summary>
    public partial class TimeOfDay
    {
        /// <summary>
        /// Initializes a new instance of the TimeOfDay class.
        /// </summary>
        /// <param name="hour">Represents the hour of the day. Value must be between 0 and 23 inclusive.</param>
        /// <param name="minute">Represents the minute of the hour. Value must be between 0 to 59 inclusive.</param>
        public TimeOfDay(
            int? hour = default(int?),
            int? minute = default(int?))
        {
            hour?.ThrowIfOutOfInclusiveRange("hour", 0, 23);
            minute?.ThrowIfOutOfInclusiveRange("minute", 0, 59);
            this.Hour = hour;
            this.Minute = minute;
        }

        /// <summary>
        /// Gets represents the hour of the day. Value must be between 0 and 23 inclusive.
        /// </summary>
        public int? Hour { get; }

        /// <summary>
        /// Gets represents the minute of the hour. Value must be between 0 to 59 inclusive.
        /// </summary>
        public int? Minute { get; }
    }
}
