// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the operation to unregister or unprovision an application type and its version that was registered with
    /// the Service Fabric.
    /// </summary>
    public partial class UnprovisionApplicationTypeDescriptionInfo
    {
        /// <summary>
        /// Initializes a new instance of the UnprovisionApplicationTypeDescriptionInfo class.
        /// </summary>
        /// <param name="applicationTypeVersion">The version of the application type as defined in the application
        /// manifest.</param>
        /// <param name="async">The flag indicating whether or not unprovision should occur asynchronously. When set to true,
        /// the unprovision operation returns when the request is accepted by the system, and the unprovision operation
        /// continues without any timeout limit. The default value is false. However, we recommend setting it to true for large
        /// application packages that were provisioned.</param>
        public UnprovisionApplicationTypeDescriptionInfo(
            string applicationTypeVersion,
            bool? async = default(bool?))
        {
            applicationTypeVersion.ThrowIfNull(nameof(applicationTypeVersion));
            this.ApplicationTypeVersion = applicationTypeVersion;
            this.Async = async;
        }

        /// <summary>
        /// Gets the version of the application type as defined in the application manifest.
        /// </summary>
        public string ApplicationTypeVersion { get; }

        /// <summary>
        /// Gets the flag indicating whether or not unprovision should occur asynchronously. When set to true, the unprovision
        /// operation returns when the request is accepted by the system, and the unprovision operation continues without any
        /// timeout limit. The default value is false. However, we recommend setting it to true for large application packages
        /// that were provisioned.
        /// </summary>
        public bool? Async { get; }
    }
}
