// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Chaos event that gets generated when Chaos is waiting for the cluster to become ready for faulting, for
    /// example, Chaos may be waiting for the on-going upgrade to finish.
    /// </summary>
    public partial class WaitingChaosEvent : ChaosEvent
    {
        /// <summary>
        /// Initializes a new instance of the WaitingChaosEvent class.
        /// </summary>
        /// <param name="timeStampUtc">The UTC timestamp when this Chaos event was generated.</param>
        /// <param name="reason">Describes why the WaitingChaosEvent was generated, for example, due to a cluster
        /// upgrade.</param>
        public WaitingChaosEvent(
            DateTime? timeStampUtc,
            string reason = default(string))
            : base(
                timeStampUtc,
                Common.ChaosEventKind.Waiting)
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets why the WaitingChaosEvent was generated, for example, due to a cluster upgrade.
        /// </summary>
        public string Reason { get; }
    }
}
