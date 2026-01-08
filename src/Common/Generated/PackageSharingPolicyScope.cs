// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for PackageSharingPolicyScope.
    /// </summary>
    public enum PackageSharingPolicyScope
    {
        /// <summary>
        /// No package sharing policy scope. The value is 0.
        /// </summary>
        None,

        /// <summary>
        /// Share all code, config and data packages from corresponding service manifest. The value is 1.
        /// </summary>
        All,

        /// <summary>
        /// Share all code packages from corresponding service manifest. The value is 2.
        /// </summary>
        Code,

        /// <summary>
        /// Share all config packages from corresponding service manifest. The value is 3.
        /// </summary>
        Config,

        /// <summary>
        /// Share all data packages from corresponding service manifest. The value is 4.
        /// </summary>
        Data,
    }
}
