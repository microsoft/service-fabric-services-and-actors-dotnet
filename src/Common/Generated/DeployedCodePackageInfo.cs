// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about code package deployed on a Service Fabric node.
    /// </summary>
    public partial class DeployedCodePackageInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedCodePackageInfo class.
        /// </summary>
        /// <param name="name">The name of the code package.</param>
        /// <param name="version">The version of the code package specified in service manifest.</param>
        /// <param name="serviceManifestName">The name of service manifest that specified this code package.</param>
        /// <param name="servicePackageActivationId">The ActivationId of a deployed service package. If
        /// ServicePackageActivationMode specified at the time of creating the service
        /// is 'SharedProcess' (or if it is not specified, in which case it defaults to 'SharedProcess'), then value of
        /// ServicePackageActivationId
        /// is always an empty string.
        /// </param>
        /// <param name="hostType">Specifies the type of host for main entry point of a code package as specified in service
        /// manifest. Possible values include: 'Invalid', 'ExeHost', 'ContainerHost'</param>
        /// <param name="hostIsolationMode">Specifies the isolation mode of main entry point of a code package when it's host
        /// type is ContainerHost. This is specified as part of container host policies in application manifest while importing
        /// service manifest. Possible values include: 'None', 'Process', 'HyperV'</param>
        /// <param name="status">Specifies the status of a deployed application or service package on a Service Fabric node.
        /// . Possible values include: 'Invalid', 'Downloading', 'Activating', 'Active', 'Upgrading', 'Deactivating',
        /// 'RanToCompletion', 'Failed'</param>
        /// <param name="runFrequencyInterval">The interval at which code package is run. This is used for periodic code
        /// package.</param>
        /// <param name="setupEntryPoint">Information about setup or main entry point of a code package deployed on a Service
        /// Fabric node.</param>
        /// <param name="mainEntryPoint">Information about setup or main entry point of a code package deployed on a Service
        /// Fabric node.</param>
        public DeployedCodePackageInfo(
            string name = default(string),
            string version = default(string),
            string serviceManifestName = default(string),
            string servicePackageActivationId = default(string),
            HostType? hostType = default(HostType?),
            HostIsolationMode? hostIsolationMode = default(HostIsolationMode?),
            DeploymentStatus? status = default(DeploymentStatus?),
            string runFrequencyInterval = default(string),
            CodePackageEntryPoint setupEntryPoint = default(CodePackageEntryPoint),
            CodePackageEntryPoint mainEntryPoint = default(CodePackageEntryPoint))
        {
            this.Name = name;
            this.Version = version;
            this.ServiceManifestName = serviceManifestName;
            this.ServicePackageActivationId = servicePackageActivationId;
            this.HostType = hostType;
            this.HostIsolationMode = hostIsolationMode;
            this.Status = status;
            this.RunFrequencyInterval = runFrequencyInterval;
            this.SetupEntryPoint = setupEntryPoint;
            this.MainEntryPoint = mainEntryPoint;
        }

        /// <summary>
        /// Gets the name of the code package.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the version of the code package specified in service manifest.
        /// </summary>
        public string Version { get; }

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
        /// Gets specifies the type of host for main entry point of a code package as specified in service manifest. Possible
        /// values include: 'Invalid', 'ExeHost', 'ContainerHost'
        /// </summary>
        public HostType? HostType { get; }

        /// <summary>
        /// Gets specifies the isolation mode of main entry point of a code package when it's host type is ContainerHost. This
        /// is specified as part of container host policies in application manifest while importing service manifest. Possible
        /// values include: 'None', 'Process', 'HyperV'
        /// </summary>
        public HostIsolationMode? HostIsolationMode { get; }

        /// <summary>
        /// Gets specifies the status of a deployed application or service package on a Service Fabric node.
        /// . Possible values include: 'Invalid', 'Downloading', 'Activating', 'Active', 'Upgrading', 'Deactivating',
        /// 'RanToCompletion', 'Failed'
        /// </summary>
        public DeploymentStatus? Status { get; }

        /// <summary>
        /// Gets the interval at which code package is run. This is used for periodic code package.
        /// </summary>
        public string RunFrequencyInterval { get; }

        /// <summary>
        /// Gets information about setup or main entry point of a code package deployed on a Service Fabric node.
        /// </summary>
        public CodePackageEntryPoint SetupEntryPoint { get; }

        /// <summary>
        /// Gets information about setup or main entry point of a code package deployed on a Service Fabric node.
        /// </summary>
        public CodePackageEntryPoint MainEntryPoint { get; }
    }
}
