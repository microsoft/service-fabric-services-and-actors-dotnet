// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes properties of a application resource.
    /// </summary>
    public partial class ApplicationProperties
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationProperties class.
        /// </summary>
        /// <param name="description">User readable description of the application.</param>
        /// <param name="services">Describes the services in the application. This property is used to create or modify
        /// services of the application. On get only the name of the service is returned. The service description can be
        /// obtained by querying for the service resource.</param>
        /// <param name="diagnostics">Describes the diagnostics definition and usage for an application resource.</param>
        /// <param name="debugParams">Internal - used by Visual Studio to setup the debugging session on the local development
        /// environment.</param>
        public ApplicationProperties(
            string description = default(string),
            IEnumerable<ServiceResourceDescription> services = default(IEnumerable<ServiceResourceDescription>),
            DiagnosticsDescription diagnostics = default(DiagnosticsDescription),
            string debugParams = default(string))
        {
            this.Description = description;
            this.Services = services;
            this.Diagnostics = diagnostics;
            this.DebugParams = debugParams;
        }

        /// <summary>
        /// Gets user readable description of the application.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the services in the application. This property is used to create or modify services of the application. On get
        /// only the name of the service is returned. The service description can be obtained by querying for the service
        /// resource.
        /// </summary>
        public IEnumerable<ServiceResourceDescription> Services { get; }

        /// <summary>
        /// Gets the diagnostics definition and usage for an application resource.
        /// </summary>
        public DiagnosticsDescription Diagnostics { get; }

        /// <summary>
        /// Gets internal - used by Visual Studio to setup the debugging session on the local development environment.
        /// </summary>
        public string DebugParams { get; }

        /// <summary>
        /// Gets names of the services in the application.
        /// </summary>
        public IEnumerable<string> ServiceNames { get; internal set; }

        /// <summary>
        /// Gets status of the application. Possible values include: 'Unknown', 'Ready', 'Upgrading', 'Creating', 'Deleting',
        /// 'Failed'
        /// 
        /// Status of the resource.
        /// </summary>
        public ResourceStatus? Status { get; internal set; }

        /// <summary>
        /// Gets additional information about the current status of the application.
        /// </summary>
        public string StatusDetails { get; internal set; }

        /// <summary>
        /// Gets the health state of an application resource. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error',
        /// 'Unknown'
        /// 
        /// The health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica etc.
        /// </summary>
        public HealthState? HealthState { get; internal set; }

        /// <summary>
        /// Gets when the application's health state is not 'Ok', this additional details from service fabric Health Manager
        /// for the user to know why the application is marked unhealthy.
        /// </summary>
        public string UnhealthyEvaluation { get; internal set; }
    }
}
