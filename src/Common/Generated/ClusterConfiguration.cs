// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the standalone cluster configuration.
    /// </summary>
    public partial class ClusterConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the ClusterConfiguration class.
        /// </summary>
        /// <param name="clusterConfigurationValue">The contents of the cluster configuration file.</param>
        public ClusterConfiguration(
            string clusterConfigurationValue = default(string))
        {
            this.ClusterConfigurationValue = clusterConfigurationValue;
        }

        /// <summary>
        /// Gets the contents of the cluster configuration file.
        /// </summary>
        public string ClusterConfigurationValue { get; }
    }
}
