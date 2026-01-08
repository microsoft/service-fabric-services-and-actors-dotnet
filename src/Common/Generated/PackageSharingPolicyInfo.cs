// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a policy for the package sharing.
    /// </summary>
    public partial class PackageSharingPolicyInfo
    {
        /// <summary>
        /// Initializes a new instance of the PackageSharingPolicyInfo class.
        /// </summary>
        /// <param name="sharedPackageName">The name of code, configuration or data package that should be shared.</param>
        /// <param name="packageSharingScope">Represents the scope for PackageSharingPolicy. This is specified during
        /// DeployServicePackageToNode operation. Possible values include: 'None', 'All', 'Code', 'Config', 'Data'</param>
        public PackageSharingPolicyInfo(
            string sharedPackageName = default(string),
            PackageSharingPolicyScope? packageSharingScope = default(PackageSharingPolicyScope?))
        {
            this.SharedPackageName = sharedPackageName;
            this.PackageSharingScope = packageSharingScope;
        }

        /// <summary>
        /// Gets the name of code, configuration or data package that should be shared.
        /// </summary>
        public string SharedPackageName { get; }

        /// <summary>
        /// Gets represents the scope for PackageSharingPolicy. This is specified during DeployServicePackageToNode operation.
        /// Possible values include: 'None', 'All', 'Code', 'Config', 'Data'
        /// </summary>
        public PackageSharingPolicyScope? PackageSharingScope { get; }
    }
}
