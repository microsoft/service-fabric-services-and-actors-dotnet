// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Azure Active Directory metadata object used for secured connection to cluster.
    /// </summary>
    public partial class AadMetadataObject
    {
        /// <summary>
        /// Initializes a new instance of the AadMetadataObject class.
        /// </summary>
        /// <param name="type">The client authentication method.</param>
        /// <param name="metadata">Azure Active Directory metadata used for secured connection to cluster.</param>
        public AadMetadataObject(
            string type = default(string),
            AadMetadata metadata = default(AadMetadata))
        {
            this.Type = type;
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the client authentication method.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets azure Active Directory metadata used for secured connection to cluster.
        /// </summary>
        public AadMetadata Metadata { get; }
    }
}
