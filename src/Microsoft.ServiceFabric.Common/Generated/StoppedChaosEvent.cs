// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Chaos event that gets generated when Chaos stops because either the user issued a stop or the time to
    /// run was up.
    /// </summary>
    public partial class StoppedChaosEvent : ChaosEvent
    {
        /// <summary>
        /// Initializes a new instance of the StoppedChaosEvent class.
        /// </summary>
        /// <param name="timeStampUtc">The UTC timestamp when this Chaos event was generated.</param>
        /// <param name="reason">Describes why Chaos stopped. Chaos can stop because of StopChaos API call or the timeToRun
        /// provided in ChaosParameters is over.</param>
        public StoppedChaosEvent(
            DateTime? timeStampUtc,
            string reason = default(string))
            : base(
                timeStampUtc,
                Common.ChaosEventKind.Stopped)
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets why Chaos stopped. Chaos can stop because of StopChaos API call or the timeToRun provided in ChaosParameters
        /// is over.
        /// </summary>
        public string Reason { get; }
    }
}
