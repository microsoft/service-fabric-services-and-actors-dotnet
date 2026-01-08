// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines description for creating a Service Fabric compose deployment.
    /// </summary>
    public partial class CreateComposeDeploymentDescription
    {
        /// <summary>
        /// Initializes a new instance of the CreateComposeDeploymentDescription class.
        /// </summary>
        /// <param name="deploymentName">The name of the deployment.</param>
        /// <param name="composeFileContent">The content of the compose file that describes the deployment to create.</param>
        /// <param name="registryCredential">Credential information to connect to container registry.</param>
        public CreateComposeDeploymentDescription(
            string deploymentName,
            string composeFileContent,
            RegistryCredential registryCredential = default(RegistryCredential))
        {
            deploymentName.ThrowIfNull(nameof(deploymentName));
            composeFileContent.ThrowIfNull(nameof(composeFileContent));
            this.DeploymentName = deploymentName;
            this.ComposeFileContent = composeFileContent;
            this.RegistryCredential = registryCredential;
        }

        /// <summary>
        /// Gets the name of the deployment.
        /// </summary>
        public string DeploymentName { get; }

        /// <summary>
        /// Gets the content of the compose file that describes the deployment to create.
        /// </summary>
        public string ComposeFileContent { get; }

        /// <summary>
        /// Gets credential information to connect to container registry.
        /// </summary>
        public RegistryCredential RegistryCredential { get; }
    }
}
