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
    /// Gets the logs from the container.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFContainerLogsMeshCodePackage")]
    public partial class GetContainerLogsMeshCodePackageCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationResourceName. The identity of the application.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string ApplicationResourceName { get; set; }

        /// <summary>
        /// Gets or sets ServiceResourceName. The identity of the service.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string ServiceResourceName { get; set; }

        /// <summary>
        /// Gets or sets ReplicaName. Service Fabric replica name.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public string ReplicaName { get; set; }

        /// <summary>
        /// Gets or sets CodePackageName. The name of code package of the service.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public string CodePackageName { get; set; }

        /// <summary>
        /// Gets or sets Tail. Number of lines to show from the end of the logs. Default is 100. 'all' to show the complete
        /// logs.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public string Tail { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var result = this.ServiceFabricClient.MeshCodePackages.GetContainerLogsAsync(
                applicationResourceName: this.ApplicationResourceName,
                serviceResourceName: this.ServiceResourceName,
                replicaName: this.ReplicaName,
                codePackageName: this.CodePackageName,
                tail: this.Tail,
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
