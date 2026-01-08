// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric config version.
    /// </summary>
    public partial class FabricConfigVersionInfo
    {
        /// <summary>
        /// Initializes a new instance of the FabricConfigVersionInfo class.
        /// </summary>
        /// <param name="configVersion">The config version of Service Fabric.</param>
        public FabricConfigVersionInfo(
            string configVersion = default(string))
        {
            this.ConfigVersion = configVersion;
        }

        /// <summary>
        /// Gets the config version of Service Fabric.
        /// </summary>
        public string ConfigVersion { get; }
    }
}
