// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Service state of Service Fabric Upgrade Orchestration Service.
    /// </summary>
    public partial class UpgradeOrchestrationServiceState
    {
        /// <summary>
        /// Initializes a new instance of the UpgradeOrchestrationServiceState class.
        /// </summary>
        /// <param name="serviceState">The state of Service Fabric Upgrade Orchestration Service.</param>
        public UpgradeOrchestrationServiceState(
            string serviceState = default(string))
        {
            this.ServiceState = serviceState;
        }

        /// <summary>
        /// Gets the state of Service Fabric Upgrade Orchestration Service.
        /// </summary>
        public string ServiceState { get; }
    }
}
