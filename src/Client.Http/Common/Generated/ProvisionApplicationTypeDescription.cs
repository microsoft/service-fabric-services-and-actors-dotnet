// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the operation to register or provision an application type using an application package uploaded to the
    /// Service Fabric image store.
    /// </summary>
    public partial class ProvisionApplicationTypeDescription : ProvisionApplicationTypeDescriptionBase
    {
        /// <summary>
        /// Initializes a new instance of the ProvisionApplicationTypeDescription class.
        /// </summary>
        /// <param name="applicationTypeBuildPath">The relative path for the application package in the image store specified
        /// during the prior upload operation.</param>
        /// <param name="async">Indicates whether or not provisioning should occur asynchronously. When set to true, the
        /// provision operation returns when the request is accepted by the system, and the provision operation continues
        /// without any timeout limit. The default value is false. For large application packages, we recommend setting the
        /// value to true.</param>
        /// <param name="applicationPackageCleanupPolicy">The kind of action that needs to be taken for cleaning up the
        /// application package after successful provision. Possible values include: 'Invalid', 'Default', 'Automatic',
        /// 'Manual'</param>
        public ProvisionApplicationTypeDescription(
            string applicationTypeBuildPath,
            bool? async = false,
            ApplicationPackageCleanupPolicy? applicationPackageCleanupPolicy = default(ApplicationPackageCleanupPolicy?))
            : base(
                Common.ProvisionApplicationTypeKind.ImageStorePath,
                async)
        {
            applicationTypeBuildPath.ThrowIfNull(nameof(applicationTypeBuildPath));
            this.ApplicationTypeBuildPath = applicationTypeBuildPath;
            this.ApplicationPackageCleanupPolicy = applicationPackageCleanupPolicy;
        }

        /// <summary>
        /// Gets the relative path for the application package in the image store specified during the prior upload operation.
        /// </summary>
        public string ApplicationTypeBuildPath { get; }

        /// <summary>
        /// Gets the kind of action that needs to be taken for cleaning up the application package after successful provision.
        /// Possible values include: 'Invalid', 'Default', 'Automatic', 'Manual'
        /// </summary>
        public ApplicationPackageCleanupPolicy? ApplicationPackageCleanupPolicy { get; }
    }
}
