// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Chaos event corresponding to a failure during validation.
    /// </summary>
    public partial class ValidationFailedChaosEvent : ChaosEvent
    {
        /// <summary>
        /// Initializes a new instance of the ValidationFailedChaosEvent class.
        /// </summary>
        /// <param name="timeStampUtc">The UTC timestamp when this Chaos event was generated.</param>
        /// <param name="reason">Describes why the ValidationFailedChaosEvent was generated. This may happen because more than
        /// MaxPercentUnhealthyNodes are unhealthy for more than MaxClusterStabilizationTimeout. This reason will be in the
        /// Reason property of the ValidationFailedChaosEvent as a string.</param>
        public ValidationFailedChaosEvent(
            DateTime? timeStampUtc,
            string reason = default(string))
            : base(
                timeStampUtc,
                Common.ChaosEventKind.ValidationFailed)
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets why the ValidationFailedChaosEvent was generated. This may happen because more than MaxPercentUnhealthyNodes
        /// are unhealthy for more than MaxClusterStabilizationTimeout. This reason will be in the Reason property of the
        /// ValidationFailedChaosEvent as a string.
        /// </summary>
        public string Reason { get; }
    }
}
