// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ProvisionApplicationTypeKind.
    /// </summary>
    public enum ProvisionApplicationTypeKind
    {
        /// <summary>
        /// Indicates that the provision kind is invalid. This value is default and should not be used. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the provision is for a package that was previously uploaded to the image store. The value is 1.
        /// </summary>
        ImageStorePath,

        /// <summary>
        /// Indicates that the provision is for an application package that was previously uploaded to an external store. The
        /// application package ends with the extension *.sfpkg. The value is 2.
        /// </summary>
        ExternalStore,
    }
}
