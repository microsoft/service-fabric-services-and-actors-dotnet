// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents health state count for entities of the specified entity kind.
    /// </summary>
    public partial class EntityKindHealthStateCount
    {
        /// <summary>
        /// Initializes a new instance of the EntityKindHealthStateCount class.
        /// </summary>
        /// <param name="entityKind">The entity kind for which health states are evaluated. Possible values include: 'Invalid',
        /// 'Node', 'Partition', 'Service', 'Application', 'Replica', 'DeployedApplication', 'DeployedServicePackage',
        /// 'Cluster'
        /// 
        /// The entity type of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica etc.
        /// </param>
        /// <param name="healthStateCount">The health state count for the entities of the specified kind.</param>
        public EntityKindHealthStateCount(
            EntityKind? entityKind = default(EntityKind?),
            HealthStateCount healthStateCount = default(HealthStateCount))
        {
            this.EntityKind = entityKind;
            this.HealthStateCount = healthStateCount;
        }

        /// <summary>
        /// Gets the entity kind for which health states are evaluated. Possible values include: 'Invalid', 'Node',
        /// 'Partition', 'Service', 'Application', 'Replica', 'DeployedApplication', 'DeployedServicePackage', 'Cluster'
        /// 
        /// The entity type of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica etc.
        /// </summary>
        public EntityKind? EntityKind { get; }

        /// <summary>
        /// Gets the health state count for the entities of the specified kind.
        /// </summary>
        public HealthStateCount HealthStateCount { get; }
    }
}
