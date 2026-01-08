// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Token Service metadata used for secured connection to cluster. For internal use only by Service Fabric tooling.
    /// </summary>
    public partial class TokenServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the TokenServiceMetadata class.
        /// </summary>
        /// <param name="metadata">The metadata for the secure token service.</param>
        /// <param name="serviceName">The Service Name.</param>
        /// <param name="serviceDnsName">The Service Dns name.</param>
        public TokenServiceMetadata(
            string metadata = default(string),
            string serviceName = default(string),
            string serviceDnsName = default(string))
        {
            this.Metadata = metadata;
            this.ServiceName = serviceName;
            this.ServiceDnsName = serviceDnsName;
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        public string Metadata { get; }

        /// <summary>
        /// Gets the Service name.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the Service DNS Name.
        /// </summary>
        public string ServiceDnsName { get; }
    }
}
