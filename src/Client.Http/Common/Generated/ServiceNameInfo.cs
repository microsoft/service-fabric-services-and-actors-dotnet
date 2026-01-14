// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the service name.
    /// </summary>
    public partial class ServiceNameInfo
    {
        /// <summary>
        /// Initializes a new instance of the ServiceNameInfo class.
        /// </summary>
        /// <param name="id">The identity of the service. This ID is an encoded representation of the service name. This is
        /// used in the REST APIs to identify the service resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the service name
        /// is "fabric:/myapp/app1/svc1",
        /// the service identity would be "myapp~app1\~svc1" in 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </param>
        /// <param name="name">The full name of the service with 'fabric:' URI scheme.</param>
        public ServiceNameInfo(
            string id = default(string),
            ServiceName name = default(ServiceName))
        {
            this.Id = id;
            this.Name = name;
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
    }
}
