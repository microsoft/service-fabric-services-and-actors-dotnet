// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for BackupEntityKind.
    /// </summary>
    public enum BackupEntityKind
    {
        /// <summary>
        /// Indicates an invalid entity kind. All Service Fabric enumerations have the invalid type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the entity is a Service Fabric partition.
        /// </summary>
        Partition,

        /// <summary>
        /// Indicates the entity is a Service Fabric service.
        /// </summary>
        Service,

        /// <summary>
        /// Indicates the entity is a Service Fabric application.
        /// </summary>
        Application,
    }
}
