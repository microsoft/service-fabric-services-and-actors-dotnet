// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for EntityKind.
    /// </summary>
    public enum EntityKind
    {
        /// <summary>
        /// Indicates an invalid entity kind. All Service Fabric enumerations have the invalid type. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates the entity is a Service Fabric node. The value is 1.
        /// </summary>
        Node,

        /// <summary>
        /// Indicates the entity is a Service Fabric partition. The value is 2.
        /// </summary>
        Partition,

        /// <summary>
        /// Indicates the entity is a Service Fabric service. The value is 3.
        /// </summary>
        Service,

        /// <summary>
        /// Indicates the entity is a Service Fabric application. The value is 4.
        /// </summary>
        Application,

        /// <summary>
        /// Indicates the entity is a Service Fabric replica. The value is 5.
        /// </summary>
        Replica,

        /// <summary>
        /// Indicates the entity is a Service Fabric deployed application. The value is 6.
        /// </summary>
        DeployedApplication,

        /// <summary>
        /// Indicates the entity is a Service Fabric deployed service package. The value is 7.
        /// </summary>
        DeployedServicePackage,

        /// <summary>
        /// Indicates the entity is a Service Fabric cluster. The value is 8.
        /// </summary>
        Cluster,
    }
}
