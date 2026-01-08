// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ApplicationDefinitionKind.
    /// </summary>
    public enum ApplicationDefinitionKind
    {
        /// <summary>
        /// Indicates the application definition kind is invalid. All Service Fabric enumerations have the invalid type. The
        /// value is 65535.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the application is defined by a Service Fabric application description. The value is 0.
        /// </summary>
        ServiceFabricApplicationDescription,

        /// <summary>
        /// Indicates the application is defined by compose file(s). The value is 1.
        /// </summary>
        Compose,
    }
}
