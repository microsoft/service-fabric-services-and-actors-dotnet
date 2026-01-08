// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ApplicationPackageCleanupPolicy.
    /// </summary>
    public enum ApplicationPackageCleanupPolicy
    {
        /// <summary>
        /// Indicates that the application package cleanup policy is invalid. This value is default. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the cleanup policy of application packages is based on the cluster setting
        /// "CleanupApplicationPackageOnProvisionSuccess." The value is 1.
        /// </summary>
        Default,

        /// <summary>
        /// Indicates that the service fabric runtime determines when to do the application package cleanup. By default,
        /// cleanup is done on successful provision. The value is 2.
        /// </summary>
        Automatic,

        /// <summary>
        /// Indicates that the user has to explicitly clean up the application package. The value is 3.
        /// </summary>
        Manual,
    }
}
