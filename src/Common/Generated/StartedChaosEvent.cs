// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Chaos event that gets generated when Chaos is started.
    /// </summary>
    public partial class StartedChaosEvent : ChaosEvent
    {
        /// <summary>
        /// Initializes a new instance of the StartedChaosEvent class.
        /// </summary>
        /// <param name="timeStampUtc">The UTC timestamp when this Chaos event was generated.</param>
        /// <param name="chaosParameters">Defines all the parameters to configure a Chaos run.
        /// </param>
        public StartedChaosEvent(
            DateTime? timeStampUtc,
            ChaosParameters chaosParameters = default(ChaosParameters))
            : base(
                timeStampUtc,
                Common.ChaosEventKind.Started)
        {
            this.ChaosParameters = chaosParameters;
        }

        /// <summary>
        /// Gets defines all the parameters to configure a Chaos run.
        /// </summary>
        public ChaosParameters ChaosParameters { get; }
    }
}
