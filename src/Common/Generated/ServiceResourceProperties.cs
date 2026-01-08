// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes properties of a service resource.
    /// </summary>
    public partial class ServiceResourceProperties
    {
        /// <summary>
        /// Initializes a new instance of the ServiceResourceProperties class.
        /// </summary>
        /// <param name="osType">The operation system required by the code in service. Possible values include: 'Linux',
        /// 'Windows'</param>
        /// <param name="codePackages">Describes the set of code packages that forms the service. A code package describes the
        /// container and the properties for running it. All the code packages are started together on the same host and share
        /// the same context (network, process etc.).
        /// </param>
        /// <param name="networkRefs">The names of the private networks that this service needs to be part of.</param>
        /// <param name="diagnostics">Reference to sinks in DiagnosticsDescription.</param>
        /// <param name="description">User readable description of the service.</param>
        /// <param name="replicaCount">The number of replicas of the service to create. Defaults to 1 if not specified.</param>
        /// <param name="executionPolicy">The execution policy of the service</param>
        /// <param name="autoScalingPolicies">Auto scaling policies</param>
        /// <param name="identityRefs">The service identity list.</param>
        /// <param name="dnsName">Dns name of the service.</param>
        public ServiceResourceProperties(
            OperatingSystemType? osType,
            IEnumerable<ContainerCodePackageProperties> codePackages,
            IEnumerable<NetworkRef> networkRefs = default(IEnumerable<NetworkRef>),
            DiagnosticsRef diagnostics = default(DiagnosticsRef),
            string description = default(string),
            int? replicaCount = default(int?),
            ExecutionPolicy executionPolicy = default(ExecutionPolicy),
            IEnumerable<AutoScalingPolicy> autoScalingPolicies = default(IEnumerable<AutoScalingPolicy>),
            IEnumerable<ServiceIdentity> identityRefs = default(IEnumerable<ServiceIdentity>),
            string dnsName = default(string))
        {
            osType.ThrowIfNull(nameof(osType));
            codePackages.ThrowIfNull(nameof(codePackages));
            this.OsType = osType;
            this.CodePackages = codePackages;
            this.NetworkRefs = networkRefs;
            this.Diagnostics = diagnostics;
            this.Description = description;
            this.ReplicaCount = replicaCount;
            this.ExecutionPolicy = executionPolicy;
            this.AutoScalingPolicies = autoScalingPolicies;
            this.IdentityRefs = identityRefs;
            this.DnsName = dnsName;
        }

        /// <summary>
        /// Gets the operation system required by the code in service. Possible values include: 'Linux', 'Windows'
        /// </summary>
        public OperatingSystemType? OsType { get; }

        /// <summary>
        /// Gets the set of code packages that forms the service. A code package the container and the properties for running
        /// it. All the code packages are started together on the same host and share the same context (network, process etc.).
        /// </summary>
        public IEnumerable<ContainerCodePackageProperties> CodePackages { get; }

        /// <summary>
        /// Gets the names of the private networks that this service needs to be part of.
        /// </summary>
        public IEnumerable<NetworkRef> NetworkRefs { get; }

        /// <summary>
        /// Gets reference to sinks in DiagnosticsDescription.
        /// </summary>
        public DiagnosticsRef Diagnostics { get; }

        /// <summary>
        /// Gets user readable description of the service.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the number of replicas of the service to create. Defaults to 1 if not specified.
        /// </summary>
        public int? ReplicaCount { get; }

        /// <summary>
        /// Gets the execution policy of the service
        /// </summary>
        public ExecutionPolicy ExecutionPolicy { get; }

        /// <summary>
        /// Gets auto scaling policies
        /// </summary>
        public IEnumerable<AutoScalingPolicy> AutoScalingPolicies { get; }

        /// <summary>
        /// Gets status of the resource. Possible values include: 'Unknown', 'Ready', 'Upgrading', 'Creating', 'Deleting',
        /// 'Failed'
        /// </summary>
        public ResourceStatus? Status { get; internal set; }

        /// <summary>
        /// Gets additional information about the current status of the service.
        /// </summary>
        public string StatusDetails { get; internal set; }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? HealthState { get; internal set; }

        /// <summary>
        /// Gets when the service's health state is not 'Ok', this additional details from service fabric Health Manager for
        /// the user to know why the service is marked unhealthy.
        /// </summary>
        public string UnhealthyEvaluation { get; internal set; }

        /// <summary>
        /// Gets the service identity list.
        /// </summary>
        public IEnumerable<ServiceIdentity> IdentityRefs { get; }

        /// <summary>
        /// Gets dns name of the service.
        /// </summary>
        public string DnsName { get; }
    }
}
