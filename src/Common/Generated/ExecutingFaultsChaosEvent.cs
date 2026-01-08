// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Chaos event that gets generated when Chaos has decided on the faults for an iteration. This Chaos event
    /// contains the details of the faults as a list of strings.
    /// </summary>
    public partial class ExecutingFaultsChaosEvent : ChaosEvent
    {
        /// <summary>
        /// Initializes a new instance of the ExecutingFaultsChaosEvent class.
        /// </summary>
        /// <param name="timeStampUtc">The UTC timestamp when this Chaos event was generated.</param>
        /// <param name="faults">List of string description of the faults that Chaos decided to execute in an
        /// iteration.</param>
        public ExecutingFaultsChaosEvent(
            DateTime? timeStampUtc,
            IEnumerable<string> faults = default(IEnumerable<string>))
            : base(
                timeStampUtc,
                Common.ChaosEventKind.ExecutingFaults)
        {
            this.Faults = faults;
        }

        /// <summary>
        /// Gets list of string description of the faults that Chaos decided to execute in an iteration.
        /// </summary>
        public IEnumerable<string> Faults { get; }
    }
}
