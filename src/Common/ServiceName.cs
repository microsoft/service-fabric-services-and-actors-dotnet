// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Linq;
    using Microsoft.ServiceFabric.Common.Resources;

    /// <summary>
    /// Represents a Service Fabric Service name.
    /// </summary>
    public sealed class ServiceName : FabricName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="serviceName">A string for the service name.</param>
        public ServiceName(string serviceName) 
            : this(new Uri(serviceName.CheckNotNull(nameof(serviceName))))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceName"/> class by using the value represented by the specified uri.
        /// </summary>
        /// <param name="serviceUri">A uri for the service name.</param>
        public ServiceName(Uri serviceUri) 
            : base(serviceUri.CheckNotNull(nameof(serviceUri)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="serviceName">A string for the service name.</param>
        /// <value>New instance of the <see cref="ServiceName"/> class.</value>
        public static implicit operator ServiceName(string serviceName) => new ServiceName(serviceName);

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceName"/> class by using the value represented by the specified uri.
        /// </summary>
        /// <param name="serviceUri">A uri for the service name.</param>
        /// <value>New instance of the <see cref="ServiceName"/> class.</value>
        public static implicit operator ServiceName(Uri serviceUri) => new ServiceName(serviceUri);
    }
}
