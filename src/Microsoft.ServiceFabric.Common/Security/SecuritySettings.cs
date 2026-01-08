// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{    
    /// <summary>
    /// An abstract base class for types that represent security settings for connecting to cluster.
    /// </summary>
    public abstract class SecuritySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecuritySettings"/> class.
        /// </summary>
        /// <param name="type">The type of security settings used to secure the Service Fabric cluster.</param>
        internal SecuritySettings(SecurityType type)
        {
            this.SecurityType = type;
        }

        /// <summary>
        /// Gets the type of security used to secure the Service Fabric cluster – valid values are "none", "x509", "Claims", "Windows".
        /// </summary>
        /// <value>
        /// The type of security to use in order to secure the cluster.
        /// </value>
        public SecurityType SecurityType { get; }
    }
}
