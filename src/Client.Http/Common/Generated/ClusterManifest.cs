// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the cluster manifest.
    /// </summary>
    public partial class ClusterManifest
    {
        /// <summary>
        /// Initializes a new instance of the ClusterManifest class.
        /// </summary>
        /// <param name="manifest">The contents of the cluster manifest file.</param>
        public ClusterManifest(
            string manifest = default(string))
        {
            this.Manifest = manifest;
        }

        /// <summary>
        /// Gets the contents of the cluster manifest file.
        /// </summary>
        public string Manifest { get; }
    }
}
