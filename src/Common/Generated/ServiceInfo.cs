// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric service.
    /// </summary>
    public abstract partial class ServiceInfo
    {
        /// <summary>
        /// Initializes a new instance of the ServiceInfo class.
        /// </summary>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
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
        protected ServiceInfo(
            ServiceKind? serviceKind,
            string id = default(string),
            ServiceName name = default(ServiceName),
            string typeName = default(string),
            string manifestVersion = default(string),
            HealthState? healthState = default(HealthState?),
            ServiceStatus? serviceStatus = default(ServiceStatus?),
            bool? isServiceGroup = default(bool?),
            ServiceMetadata serviceMetadata = default(ServiceMetadata))
        {
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceKind = serviceKind;
            this.Id = id;
            this.Name = name;
            this.TypeName = typeName;
            this.ManifestVersion = manifestVersion;
            this.HealthState = healthState;
            this.ServiceStatus = serviceStatus;
            this.IsServiceGroup = isServiceGroup;
            this.ServiceMetadata = serviceMetadata;
        }

        /// <summary>
        /// Gets the identity of the service. This ID is an encoded representation of the service name. This is used in the
        /// REST APIs to identify the service resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the service name
        /// is "fabric:/myapp/app1/svc1",
        /// the service identity would be "myapp~app1\~svc1" in 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the full name of the service with 'fabric:' URI scheme.
        /// </summary>
        public ServiceName Name { get; }

        /// <summary>
        /// Gets name of the service type as specified in the service manifest.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Gets the version of the service manifest.
        /// </summary>
        public string ManifestVersion { get; }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? HealthState { get; }

        /// <summary>
        /// Gets the status of the application. Possible values include: 'Unknown', 'Active', 'Upgrading', 'Deleting',
        /// 'Creating', 'Failed'
        /// </summary>
        public ServiceStatus? ServiceStatus { get; }

        /// <summary>
        /// Gets whether the service is in a service group.
        /// </summary>
        public bool? IsServiceGroup { get; }

        /// <summary>
        /// Gets metadata associated with a specific service.
        /// </summary>
        public ServiceMetadata ServiceMetadata { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
