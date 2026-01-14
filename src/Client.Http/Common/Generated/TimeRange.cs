// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a time range in a 24 hour day specified by a start and end time.
    /// </summary>
    public partial class TimeRange
    {
        /// <summary>
        /// Initializes a new instance of the TimeRange class.
        /// </summary>
        /// <param name="startTime">Defines an hour and minute of the day specified in 24 hour time.</param>
        /// <param name="endTime">Defines an hour and minute of the day specified in 24 hour time.</param>
        public TimeRange(
            TimeOfDay startTime = default(TimeOfDay),
            TimeOfDay endTime = default(TimeOfDay))
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        /// <summary>
        /// Gets defines an hour and minute of the day specified in 24 hour time.
        /// </summary>
        public TimeOfDay StartTime { get; }

        /// <summary>
        /// Gets defines an hour and minute of the day specified in 24 hour time.
        /// </summary>
        public TimeOfDay EndTime { get; }
    }
}
