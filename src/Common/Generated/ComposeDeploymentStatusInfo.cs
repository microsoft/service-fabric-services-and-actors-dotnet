// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric compose deployment.
    /// </summary>
    public partial class ComposeDeploymentStatusInfo
    {
        /// <summary>
        /// Initializes a new instance of the ComposeDeploymentStatusInfo class.
        /// </summary>
        /// <param name="name">The name of the deployment.</param>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="status">The status of the compose deployment. Possible values include: 'Invalid', 'Provisioning',
        /// 'Creating', 'Ready', 'Unprovisioning', 'Deleting', 'Failed', 'Upgrading'</param>
        /// <param name="statusDetails">The status details of compose deployment including failure message.</param>
        public ComposeDeploymentStatusInfo(
            string name = default(string),
            ApplicationName applicationName = default(ApplicationName),
            ComposeDeploymentStatus? status = default(ComposeDeploymentStatus?),
            string statusDetails = default(string))
        {
            this.Name = name;
            this.ApplicationName = applicationName;
            this.Status = status;
            this.StatusDetails = statusDetails;
        }

        /// <summary>
        /// Gets the name of the deployment.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }

        /// <summary>
        /// Gets the status of the compose deployment. Possible values include: 'Invalid', 'Provisioning', 'Creating', 'Ready',
        /// 'Unprovisioning', 'Deleting', 'Failed', 'Upgrading'
        /// </summary>
        public ComposeDeploymentStatus? Status { get; }

        /// <summary>
        /// Gets the status details of compose deployment including failure message.
        /// </summary>
        public string StatusDetails { get; }
    }
}
