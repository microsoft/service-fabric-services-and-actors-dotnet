// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Response body that wraps container API result.
    /// </summary>
    public partial class ContainerApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the ContainerApiResponse class.
        /// </summary>
        /// <param name="containerApiResult">Container API result.</param>
        public ContainerApiResponse(
            ContainerApiResult containerApiResult)
        {
            containerApiResult.ThrowIfNull(nameof(containerApiResult));
            this.ContainerApiResult = containerApiResult;
        }

        /// <summary>
        /// Gets container API result.
        /// </summary>
        public ContainerApiResult ContainerApiResult { get; }
    }
}
