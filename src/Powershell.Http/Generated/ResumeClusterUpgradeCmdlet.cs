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
    /// Make the cluster upgrade move on to the next upgrade domain.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Resume, "SFClusterUpgrade")]
    public partial class ResumeClusterUpgradeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets UpgradeDomain. The next upgrade domain for this cluster upgrade.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string UpgradeDomain { get; set; }

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
            var resumeClusterUpgradeDescription = new ResumeClusterUpgradeDescription(
            upgradeDomain: this.UpgradeDomain);

            this.ServiceFabricClient.Cluster.ResumeClusterUpgradeAsync(
                resumeClusterUpgradeDescription: resumeClusterUpgradeDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
