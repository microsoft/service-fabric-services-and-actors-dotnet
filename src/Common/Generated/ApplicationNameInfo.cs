// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the application name.
    /// </summary>
    public partial class ApplicationNameInfo
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationNameInfo class.
        /// </summary>
        /// <param name="id">The identity of the application. This is an encoded representation of the application name. This
        /// is used in the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name="name">The name of the application, including the 'fabric:' URI scheme.</param>
        public ApplicationNameInfo(
            string id = default(string),
            ApplicationName name = default(ApplicationName))
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets the identity of the application. This is an encoded representation of the application name. This is used in
        /// the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName Name { get; }
    }
}
