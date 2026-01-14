// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines description for restarting a deployed code package on Service Fabric node.
    /// </summary>
    public partial class RestartDeployedCodePackageDescription
    {
        /// <summary>
        /// Initializes a new instance of the RestartDeployedCodePackageDescription class.
        /// </summary>
        /// <param name="serviceManifestName">The name of service manifest that specified this code package.</param>
        /// <param name="codePackageName">The name of the code package defined in the service manifest.</param>
        /// <param name="codePackageInstanceId">The instance ID for currently running entry point. For a code package setup
        /// entry point (if specified) runs first and after it finishes main entry point is started.
        /// Each time entry point executable is run, its instance ID will change. If 0 is passed in as the code package
        /// instance ID, the API will restart the code package with whatever instance ID it is currently running.
        /// If an instance ID other than 0 is passed in, the API will restart the code package only if the current Instance ID
        /// matches the passed in instance ID.
        /// Note, passing in the exact instance ID (not 0) in the API is safer, because if ensures at most one restart of the
        /// code package.
        /// </param>
        /// <param name="servicePackageActivationId">The ActivationId of a deployed service package. If
        /// ServicePackageActivationMode specified at the time of creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </param>
        public RestartDeployedCodePackageDescription(
            string serviceManifestName,
            string codePackageName,
            string codePackageInstanceId,
            string servicePackageActivationId = default(string))
        {
            serviceManifestName.ThrowIfNull(nameof(serviceManifestName));
            codePackageName.ThrowIfNull(nameof(codePackageName));
            codePackageInstanceId.ThrowIfNull(nameof(codePackageInstanceId));
            this.ServiceManifestName = serviceManifestName;
            this.CodePackageName = codePackageName;
            this.CodePackageInstanceId = codePackageInstanceId;
            this.ServicePackageActivationId = servicePackageActivationId;
        }

        /// <summary>
        /// Gets the name of service manifest that specified this code package.
        /// </summary>
        public string ServiceManifestName { get; }

        /// <summary>
        /// Gets the ActivationId of a deployed service package. If ServicePackageActivationMode specified at the time of
        /// creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </summary>
        public string ServicePackageActivationId { get; }

        /// <summary>
        /// Gets the name of the code package defined in the service manifest.
        /// </summary>
        public string CodePackageName { get; }

        /// <summary>
        /// Gets the instance ID for currently running entry point. For a code package setup entry point (if specified) runs
        /// first and after it finishes main entry point is started.
        /// Each time entry point executable is run, its instance ID will change. If 0 is passed in as the code package
        /// instance ID, the API will restart the code package with whatever instance ID it is currently running.
        /// If an instance ID other than 0 is passed in, the API will restart the code package only if the current Instance ID
        /// matches the passed in instance ID.
        /// Note, passing in the exact instance ID (not 0) in the API is safer, because if ensures at most one restart of the
        /// code package.
        /// </summary>
        public string CodePackageInstanceId { get; }
    }
}
