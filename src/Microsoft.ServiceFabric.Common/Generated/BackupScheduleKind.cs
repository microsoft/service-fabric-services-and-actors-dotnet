// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for BackupScheduleKind.
    /// </summary>
    public enum BackupScheduleKind
    {
        /// <summary>
        /// Indicates an invalid backup schedule kind. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates a time-based backup schedule.
        /// </summary>
        TimeBased,

        /// <summary>
        /// Indicates a frequency-based backup schedule.
        /// </summary>
        FrequencyBased,
    }
}
