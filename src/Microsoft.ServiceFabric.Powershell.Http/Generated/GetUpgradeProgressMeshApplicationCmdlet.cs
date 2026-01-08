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
    /// Gets the progress of the latest upgrade performed on this application resource.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFUpgradeProgressMeshApplication")]
    public partial class GetUpgradeProgressMeshApplicationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationResourceName. The identity of the application.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string ApplicationResourceName { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var result = this.ServiceFabricClient.MeshApplications.GetUpgradeProgressAsync(
                applicationResourceName: this.ApplicationResourceName,
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
