// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes destination endpoint for routing traffic.
    /// </summary>
    public partial class GatewayDestination
    {
        /// <summary>
        /// Initializes a new instance of the GatewayDestination class.
        /// </summary>
        /// <param name="applicationName">Name of the service fabric Mesh application.</param>
        /// <param name="serviceName">service that contains the endpoint.</param>
        /// <param name="endpointName">name of the endpoint in the service.</param>
        public GatewayDestination(
            string applicationName,
            string serviceName,
            string endpointName)
        {
            applicationName.ThrowIfNull(nameof(applicationName));
            serviceName.ThrowIfNull(nameof(serviceName));
            endpointName.ThrowIfNull(nameof(endpointName));
            this.ApplicationName = applicationName;
            this.ServiceName = serviceName;
            this.EndpointName = endpointName;
        }

        /// <summary>
        /// Gets name of the service fabric Mesh application.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets service that contains the endpoint.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets name of the endpoint in the service.
        /// </summary>
        public string EndpointName { get; }
    }
}
