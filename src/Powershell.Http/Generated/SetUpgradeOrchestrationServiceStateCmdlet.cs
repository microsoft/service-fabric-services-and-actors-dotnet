// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Update the service state of Service Fabric Upgrade Orchestration Service.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "SFUpgradeOrchestrationServiceState")]
    public partial class SetUpgradeOrchestrationServiceStateCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ServiceState. The state of Service Fabric Upgrade Orchestration Service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string ServiceState { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var upgradeOrchestrationServiceState = new UpgradeOrchestrationServiceState(
            serviceState: this.ServiceState);

            var result = this.ServiceFabricClient.Cluster.SetUpgradeOrchestrationServiceStateAsync(
                upgradeOrchestrationServiceState: upgradeOrchestrationServiceState,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                this.WriteObject(this.FormatOutput(result));
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
