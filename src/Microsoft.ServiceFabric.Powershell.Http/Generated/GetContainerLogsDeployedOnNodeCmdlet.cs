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
    /// Gets the container logs for container deployed on a Service Fabric node.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFContainerLogsDeployedOnNode")]
    public partial class GetContainerLogsDeployedOnNodeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets ServiceManifestName. The name of a service manifest registered as part of an application type in a
        /// Service Fabric cluster.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2)]
        public string ServiceManifestName { get; set; }

        /// <summary>
        /// Gets or sets CodePackageName. The name of code package specified in service manifest registered as part of an
        /// application type in a Service Fabric cluster.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 3)]
        public string CodePackageName { get; set; }

        /// <summary>
        /// Gets or sets Tail. Number of lines to show from the end of the logs. Default is 100. 'all' to show the complete
        /// logs.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public string Tail { get; set; }

        /// <summary>
        /// Gets or sets Previous. Specifies whether to get container logs from exited/dead containers of the code package
        /// instance.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public bool? Previous { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var result = this.ServiceFabricClient.CodePackages.GetContainerLogsDeployedOnNodeAsync(
                nodeName: this.NodeName,
                applicationId: this.ApplicationId,
                serviceManifestName: this.ServiceManifestName,
                codePackageName: this.CodePackageName,
                tail: this.Tail,
                previous: this.Previous,
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
