// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for provisioning a cluster.
    /// </summary>
    public partial class ProvisionFabricDescription
    {
        /// <summary>
        /// Initializes a new instance of the ProvisionFabricDescription class.
        /// </summary>
        /// <param name="codeFilePath">The cluster code package file path.</param>
        /// <param name="clusterManifestFilePath">The cluster manifest file path.</param>
        public ProvisionFabricDescription(
            string codeFilePath = default(string),
            string clusterManifestFilePath = default(string))
        {
            this.CodeFilePath = codeFilePath;
            this.ClusterManifestFilePath = clusterManifestFilePath;
        }

        /// <summary>
        /// Gets the cluster code package file path.
        /// </summary>
        public string CodeFilePath { get; }

        /// <summary>
        /// Gets the cluster manifest file path.
        /// </summary>
        public string ClusterManifestFilePath { get; }
    }
}
