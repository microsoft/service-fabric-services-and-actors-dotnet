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
    /// Restarts a code package deployed on a Service Fabric node in a cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Restart, "SFDeployedCodePackage")]
    public partial class RestartDeployedCodePackageCmdlet : CommonCmdletBase
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
        /// Gets or sets ServiceManifestName. The name of service manifest that specified this code package.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public string ServiceManifestName { get; set; }

        /// <summary>
        /// Gets or sets CodePackageName. The name of the code package defined in the service manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public string CodePackageName { get; set; }

        /// <summary>
        /// Gets or sets CodePackageInstanceId. The instance ID for currently running entry point. For a code package setup
        /// entry point (if specified) runs first and after it finishes main entry point is started.
        /// Each time entry point executable is run, its instance ID will change. If 0 is passed in as the code package
        /// instance ID, the API will restart the code package with whatever instance ID it is currently running.
        /// If an instance ID other than 0 is passed in, the API will restart the code package only if the current Instance ID
        /// matches the passed in instance ID.
        /// Note, passing in the exact instance ID (not 0) in the API is safer, because if ensures at most one restart of the
        /// code package.
        /// </summary>
        [Parameter(Mandatory = true, Position = 4)]
        public string CodePackageInstanceId { get; set; }

        /// <summary>
        /// Gets or sets ServicePackageActivationId. The ActivationId of a deployed service package. If
        /// ServicePackageActivationMode specified at the time of creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public string ServicePackageActivationId { get; set; }

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
            var restartDeployedCodePackageDescription = new RestartDeployedCodePackageDescription(
            serviceManifestName: this.ServiceManifestName,
            codePackageName: this.CodePackageName,
            codePackageInstanceId: this.CodePackageInstanceId,
            servicePackageActivationId: this.ServicePackageActivationId);

            this.ServiceFabricClient.CodePackages.RestartDeployedCodePackageAsync(
                nodeName: this.NodeName,
                applicationId: this.ApplicationId,
                restartDeployedCodePackageDescription: restartDeployedCodePackageDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
