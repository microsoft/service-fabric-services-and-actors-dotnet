// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Azure Active Directory metadata used for secured connection to cluster.
    /// </summary>
    public partial class AadMetadata
    {
        /// <summary>
        /// Initializes a new instance of the AadMetadata class.
        /// </summary>
        /// <param name="authority">The AAD authority url.</param>
        /// <param name="client">The AAD client application Id.</param>
        /// <param name="cluster">The AAD cluster application Id.</param>
        /// <param name="login">The AAD login url.</param>
        /// <param name="redirect">The client application redirect address.</param>
        /// <param name="tenant">The AAD tenant Id.</param>
        public AadMetadata(
            string authority = default(string),
            string client = default(string),
            string cluster = default(string),
            string login = default(string),
            string redirect = default(string),
            string tenant = default(string))
        {
            this.Authority = authority;
            this.Client = client;
            this.Cluster = cluster;
            this.Login = login;
            this.Redirect = redirect;
            this.Tenant = tenant;
        }

        /// <summary>
        /// Gets the AAD authority url.
        /// </summary>
        public string Authority { get; }

        /// <summary>
        /// Gets the AAD client application Id.
        /// </summary>
        public string Client { get; }

        /// <summary>
        /// Gets the AAD cluster application Id.
        /// </summary>
        public string Cluster { get; }

        /// <summary>
        /// Gets the AAD login url.
        /// </summary>
        public string Login { get; }

        /// <summary>
        /// Gets the client application redirect address.
        /// </summary>
        public string Redirect { get; }

        /// <summary>
        /// Gets the AAD tenant Id.
        /// </summary>
        public string Tenant { get; }
    }
}
