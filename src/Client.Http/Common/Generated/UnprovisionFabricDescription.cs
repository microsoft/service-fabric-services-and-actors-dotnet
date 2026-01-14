// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for unprovisioning a cluster.
    /// </summary>
    public partial class UnprovisionFabricDescription
    {
        /// <summary>
        /// Initializes a new instance of the UnprovisionFabricDescription class.
        /// </summary>
        /// <param name="codeVersion">The cluster code package version.</param>
        /// <param name="configVersion">The cluster manifest version.</param>
        public UnprovisionFabricDescription(
            string codeVersion = default(string),
            string configVersion = default(string))
        {
            this.CodeVersion = codeVersion;
            this.ConfigVersion = configVersion;
        }

        /// <summary>
        /// Gets the cluster code package version.
        /// </summary>
        public string CodeVersion { get; }

        /// <summary>
        /// Gets the cluster manifest version.
        /// </summary>
        public string ConfigVersion { get; }
    }
}
