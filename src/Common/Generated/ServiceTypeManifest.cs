// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains the manifest describing a service type registered as part of an application in a Service Fabric cluster.
    /// </summary>
    public partial class ServiceTypeManifest
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTypeManifest class.
        /// </summary>
        /// <param name="manifest">The XML manifest as a string.</param>
        public ServiceTypeManifest(
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
