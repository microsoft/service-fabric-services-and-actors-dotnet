// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The REST API operations for Service Fabric return standard HTTP status codes. This type defines the additional
    /// information returned from the Service Fabric API operations that are not successful.
    /// </summary>
    public partial class FabricError
    {
        /// <summary>
        /// Initializes a new instance of the FabricError class.
        /// </summary>
        /// <param name="error">Error object containing error code and error message.</param>
        public FabricError(
            FabricErrorError error)
        {
            error.ThrowIfNull(nameof(error));
            this.Error = error;
        }

        /// <summary>
        /// Gets error object containing error code and error message.
        /// </summary>
        public FabricErrorError Error { get; }
    }
}
