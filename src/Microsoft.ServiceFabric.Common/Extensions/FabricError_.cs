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
        /*
            Add Properties to expose Code and Message from FabricError. 
        */

        /// <summary>
        /// Gets ErrorCode.
        /// </summary>
        public FabricErrorCodes? ErrorCode => this.Error.Code;

        /// <summary>
        /// Gets error message.
        /// </summary>
        public string Message => this.Error.Message;
    }
}
