// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ApplicationTypeDefinitionKind.
    /// </summary>
    public enum ApplicationTypeDefinitionKind
    {
        /// <summary>
        /// Indicates the application type definition kind is invalid. All Service Fabric enumerations have the invalid type.
        /// The value is 0.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the application type is defined and created by a Service Fabric application package provided by the user.
        /// The value is 1.
        /// </summary>
        ServiceFabricApplicationPackage,

        /// <summary>
        /// Indicates the application type is defined and created implicitly as part of a compose deployment. The value is 2.
        /// </summary>
        Compose,
    }
}
