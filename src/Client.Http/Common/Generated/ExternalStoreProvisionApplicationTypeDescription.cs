// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the operation to register or provision an application type using an application package from an external
    /// store instead of a package uploaded to the Service Fabric image store.
    /// </summary>
    public partial class ExternalStoreProvisionApplicationTypeDescription : ProvisionApplicationTypeDescriptionBase
    {
        /// <summary>
        /// Initializes a new instance of the ExternalStoreProvisionApplicationTypeDescription class.
        /// </summary>
        /// <param name="applicationPackageDownloadUri">The path to the '.sfpkg' application package from where the application
        /// package can be downloaded using HTTP or HTTPS protocols. The application package can be stored in an external store
        /// that provides GET operation to download the file. Supported protocols are HTTP and HTTPS, and the path must allow
        /// READ access.</param>
        /// <param name="applicationTypeName">The application type name represents the name of the application type found in
        /// the application manifest.</param>
        /// <param name="applicationTypeVersion">The application type version represents the version of the application type
        /// found in the application manifest.</param>
        /// <param name="async">Indicates whether or not provisioning should occur asynchronously. When set to true, the
        /// provision operation returns when the request is accepted by the system, and the provision operation continues
        /// without any timeout limit. The default value is false. For large application packages, we recommend setting the
        /// value to true.</param>
        public ExternalStoreProvisionApplicationTypeDescription(
            string applicationPackageDownloadUri,
            string applicationTypeName,
            string applicationTypeVersion,
            bool? async = false)
            : base(
                Common.ProvisionApplicationTypeKind.ExternalStore,
                async)
        {
            applicationPackageDownloadUri.ThrowIfNull(nameof(applicationPackageDownloadUri));
            applicationTypeName.ThrowIfNull(nameof(applicationTypeName));
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            this.ApplicationPackageDownloadUri = applicationPackageDownloadUri;
            this.ApplicationTypeName = applicationTypeName;
            this.ApplicationTypeVersion = applicationTypeVersion;
        }

        /// <summary>
        /// Gets the path to the '.sfpkg' application package from where the application package can be downloaded using HTTP
        /// or HTTPS protocols. The application package can be stored in an external store that provides GET operation to
        /// download the file. Supported protocols are HTTP and HTTPS, and the path must allow READ access.
        /// </summary>
        public string ApplicationPackageDownloadUri { get; }

        /// <summary>
        /// Gets the application type name represents the name of the application type found in the application manifest.
        /// </summary>
        public string ApplicationTypeName { get; }

        /// <summary>
        /// Gets the application type version represents the version of the application type found in the application manifest.
        /// </summary>
        public string ApplicationTypeVersion { get; }
    }
}
