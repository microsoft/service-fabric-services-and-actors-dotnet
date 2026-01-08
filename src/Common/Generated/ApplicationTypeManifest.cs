// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains the manifest describing an application type registered in a Service Fabric cluster.
    /// </summary>
    public partial class ApplicationTypeManifest
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationTypeManifest class.
        /// </summary>
        /// <param name="manifest">The XML manifest as a string.</param>
        public ApplicationTypeManifest(
            string manifest = default(string))
        {
            this.Manifest = manifest;
        }

        /// <summary>
        /// Gets the XML manifest as a string.
        /// </summary>
        public string Manifest { get; }
    }
}
