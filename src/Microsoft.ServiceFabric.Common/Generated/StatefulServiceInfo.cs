// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a stateful Service Fabric service.
    /// </summary>
    public partial class StatefulServiceInfo : ServiceInfo
    {
        /// <summary>
        /// Initializes a new instance of the StatefulServiceInfo class.
        /// </summary>
        /// <param name="id">The identity of the service. This ID is an encoded representation of the service name. This is
        /// used in the REST APIs to identify the service resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the service name
        /// is "fabric:/myapp/app1/svc1",
        /// the service identity would be "myapp~app1\~svc1" in 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name="name">The full name of the service with 'fabric:' URI scheme.</param>
        /// <param name="typeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="manifestVersion">The version of the service manifest.</param>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="serviceStatus">The status of the application. Possible values include: 'Unknown', 'Active',
        /// 'Upgrading', 'Deleting', 'Creating', 'Failed'</param>
        /// <param name="isServiceGroup">Whether the service is in a service group.</param>
        /// <param name="serviceMetadata">Metadata associated with a specific service.</param>
        /// <param name="hasPersistedState">Whether the service has persisted state.</param>
        public StatefulServiceInfo(
            string id = default(string),
            ServiceName name = default(ServiceName),
            string typeName = default(string),
            string manifestVersion = default(string),
            HealthState? healthState = default(HealthState?),
            ServiceStatus? serviceStatus = default(ServiceStatus?),
            bool? isServiceGroup = default(bool?),
            ServiceMetadata serviceMetadata = default(ServiceMetadata),
            bool? hasPersistedState = default(bool?))
            : base(
                Common.ServiceKind.Stateful,
                id,
                name,
                typeName,
                manifestVersion,
                healthState,
                serviceStatus,
                isServiceGroup,
                serviceMetadata)
        {
            this.HasPersistedState = hasPersistedState;
        }

        /// <summary>
        /// Gets whether the service has persisted state.
        /// </summary>
        public bool? HasPersistedState { get; }
    }
}
