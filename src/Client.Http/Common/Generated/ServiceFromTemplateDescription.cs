// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines description for creating a Service Fabric service from a template defined in the application manifest.
    /// </summary>
    public partial class ServiceFromTemplateDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServiceFromTemplateDescription class.
        /// </summary>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="serviceName">The full name of the service with 'fabric:' URI scheme.</param>
        /// <param name="serviceTypeName">Name of the service type as specified in the service manifest.</param>
        /// <param name="initializationData">The initialization data for the newly created service instance.</param>
        /// <param name="servicePackageActivationMode">The activation mode of service package to be used for a service.
        /// Possible values include: 'SharedProcess', 'ExclusiveProcess'
        /// 
        /// The activation mode of service package to be used for a Service Fabric service. This is specified at the time of
        /// creating the Service.
        /// </param>
        /// <param name="serviceDnsName">The DNS name of the service. It requires the DNS system service to be enabled in
        /// Service Fabric cluster.</param>
        public ServiceFromTemplateDescription(
            ApplicationName applicationName,
            ServiceName serviceName,
            string serviceTypeName,
            byte[] initializationData = default(byte[]),
            ServicePackageActivationMode? servicePackageActivationMode = default(ServicePackageActivationMode?),
            string serviceDnsName = default(string))
        {
            applicationName.ThrowIfNull(nameof(applicationName));
            serviceName.ThrowIfNull(nameof(serviceName));
            serviceTypeName.ThrowIfNull(nameof(serviceTypeName));
            this.ApplicationName = applicationName;
            this.ServiceName = serviceName;
            this.ServiceTypeName = serviceTypeName;
            this.InitializationData = initializationData;
            this.ServicePackageActivationMode = servicePackageActivationMode;
            this.ServiceDnsName = serviceDnsName;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }

        /// <summary>
        /// Gets the full name of the service with 'fabric:' URI scheme.
        /// </summary>
        public ServiceName ServiceName { get; }

        /// <summary>
        /// Gets name of the service type as specified in the service manifest.
        /// </summary>
        public string ServiceTypeName { get; }

        /// <summary>
        /// Gets the initialization data for the newly created service instance.
        /// </summary>
        public byte[] InitializationData { get; }

        /// <summary>
        /// Gets the activation mode of service package to be used for a service. Possible values include: 'SharedProcess',
        /// 'ExclusiveProcess'
        /// 
        /// The activation mode of service package to be used for a Service Fabric service. This is specified at the time of
        /// creating the Service.
        /// </summary>
        public ServicePackageActivationMode? ServicePackageActivationMode { get; }

        /// <summary>
        /// Gets the DNS name of the service. It requires the DNS system service to be enabled in Service Fabric cluster.
        /// </summary>
        public string ServiceDnsName { get; }
    }
}
