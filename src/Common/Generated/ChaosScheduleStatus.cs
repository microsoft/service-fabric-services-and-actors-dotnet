// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ChaosScheduleStatus.
    /// </summary>
    public enum ChaosScheduleStatus
    {
        /// <summary>
        /// Indicates an invalid Chaos Schedule status. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the schedule is stopped and not being used to schedule runs of chaos. The value is one.
        /// </summary>
        Stopped,

        /// <summary>
        /// Indicates that the schedule is active and is being used to schedule runs of Chaos. The value is two.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the schedule is expired and will no longer be used to schedule runs of Chaos. The value is three.
        /// </summary>
        Expired,

        /// <summary>
        /// Indicates that the schedule is pending and is not yet being used to schedule runs of Chaos but will be used when
        /// the start time is passed. The value is four.
        /// </summary>
        Pending,
    }
}
