// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric code version.
    /// </summary>
    public partial class FabricCodeVersionInfo
    {
        /// <summary>
        /// Initializes a new instance of the FabricCodeVersionInfo class.
        /// </summary>
        /// <param name="codeVersion">The product version of Service Fabric.</param>
        public FabricCodeVersionInfo(
            string codeVersion = default(string))
        {
            this.CodeVersion = codeVersion;
        }

        /// <summary>
        /// Gets the product version of Service Fabric.
        /// </summary>
        public string CodeVersion { get; }
    }
}
