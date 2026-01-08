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
    /// Creates a Service Fabric compose deployment.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFComposeDeployment")]
    public partial class NewComposeDeploymentCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets DeploymentName. The name of the deployment.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string DeploymentName { get; set; }

        /// <summary>
        /// Gets or sets ComposeFileContent. The content of the compose file that describes the deployment to create.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string ComposeFileContent { get; set; }

        /// <summary>
        /// Gets or sets RegistryUserName. The user name to connect to container registry.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public string RegistryUserName { get; set; }

        /// <summary>
        /// Gets or sets RegistryPassword. The password for supplied username to connect to container registry.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public string RegistryPassword { get; set; }

        /// <summary>
        /// Gets or sets PasswordEncrypted. Indicates that supplied container registry password is encrypted.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public bool? PasswordEncrypted { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var registryCredential = new RegistryCredential(
            registryUserName: this.RegistryUserName,
            registryPassword: this.RegistryPassword,
            passwordEncrypted: this.PasswordEncrypted);

            var createComposeDeploymentDescription = new CreateComposeDeploymentDescription(
            deploymentName: this.DeploymentName,
            composeFileContent: this.ComposeFileContent,
            registryCredential: registryCredential);

            this.ServiceFabricClient.ComposeDeployments.CreateComposeDeploymentAsync(
                createComposeDeploymentDescription: createComposeDeploymentDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
