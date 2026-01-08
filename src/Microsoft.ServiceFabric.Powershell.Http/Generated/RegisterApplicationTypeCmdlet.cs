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
    /// Provisions or registers a Service Fabric application type with the cluster using the '.sfpkg' package in the
    /// external store or using the application package in the image store.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Register, "SFApplicationType")]
    public partial class RegisterApplicationTypeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ImageStorePath flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_ImageStorePath_")]
        public SwitchParameter ImageStorePath { get; set; }

        /// <summary>
        /// Gets or sets ExternalStore flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_ExternalStore_")]
        public SwitchParameter ExternalStore { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeBuildPath. The relative path for the application package in the image store specified
        /// during the prior upload operation.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "_ImageStorePath_")]
        public string ApplicationTypeBuildPath { get; set; }

        /// <summary>
        /// Gets or sets ApplicationPackageDownloadUri. The path to the '.sfpkg' application package from where the application
        /// package can be downloaded using HTTP or HTTPS protocols. The application package can be stored in an external store
        /// that provides GET operation to download the file. Supported protocols are HTTP and HTTPS, and the path must allow
        /// READ access.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "_ExternalStore_")]
        public string ApplicationPackageDownloadUri { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeName. The application type name represents the name of the application type found in
        /// the application manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_ExternalStore_")]
        public string ApplicationTypeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeVersion. The application type version represents the version of the application type
        /// found in the application manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 4, ParameterSetName = "_ExternalStore_")]
        public string ApplicationTypeVersion { get; set; }

        /// <summary>
        /// Gets or sets Async. Indicates whether or not provisioning should occur asynchronously. When set to true, the
        /// provision operation returns when the request is accepted by the system, and the provision operation continues
        /// without any timeout limit. The default value is false. For large application packages, we recommend setting the
        /// value to true.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public bool? Async { get; set; } = false;

        /// <summary>
        /// Gets or sets ApplicationPackageCleanupPolicy. The kind of action that needs to be taken for cleaning up the
        /// application package after successful provision. Possible values include: 'Invalid', 'Default', 'Automatic',
        /// 'Manual'
        /// </summary>
        [Parameter(Mandatory = false, Position = 6, ParameterSetName = "_ImageStorePath_")]
        public ApplicationPackageCleanupPolicy? ApplicationPackageCleanupPolicy { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            ProvisionApplicationTypeDescriptionBase provisionApplicationTypeDescriptionBase = null;
            if (this.ImageStorePath.IsPresent)
            {
                provisionApplicationTypeDescriptionBase = new ProvisionApplicationTypeDescription(
                    applicationTypeBuildPath: this.ApplicationTypeBuildPath,
                    async: this.Async,
                    applicationPackageCleanupPolicy: this.ApplicationPackageCleanupPolicy);
            }
            else if (this.ExternalStore.IsPresent)
            {
                provisionApplicationTypeDescriptionBase = new ExternalStoreProvisionApplicationTypeDescription(
                    applicationPackageDownloadUri: this.ApplicationPackageDownloadUri,
                    applicationTypeName: this.ApplicationTypeName,
                    applicationTypeVersion: this.ApplicationTypeVersion,
                    async: this.Async);
            }

            this.ServiceFabricClient.ApplicationTypes.ProvisionApplicationTypeAsync(
                provisionApplicationTypeDescription: provisionApplicationTypeDescriptionBase,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
