// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The cluster version.
    /// </summary>
    public partial class ClusterVersion
    {
        /// <summary>
        /// Initializes a new instance of the ClusterVersion class.
        /// </summary>
        /// <param name="version">The Service Fabric cluster runtime version.</param>
        public ClusterVersion(
            string version = default(string))
        {
            this.Version = version;
        }

        /// <summary>
        /// Gets the Service Fabric cluster runtime version.
        /// </summary>
        public string Version { get; }
    }
}
