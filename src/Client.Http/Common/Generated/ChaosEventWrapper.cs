// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper object for Chaos event.
    /// </summary>
    public partial class ChaosEventWrapper
    {
        /// <summary>
        /// Initializes a new instance of the ChaosEventWrapper class.
        /// </summary>
        /// <param name="chaosEvent">Represents an event generated during a Chaos run.</param>
        public ChaosEventWrapper(
            ChaosEvent chaosEvent = default(ChaosEvent))
        {
            this.ChaosEvent = chaosEvent;
        }

        /// <summary>
        /// Gets represents an event generated during a Chaos run.
        /// </summary>
        public ChaosEvent ChaosEvent { get; }
    }
}
