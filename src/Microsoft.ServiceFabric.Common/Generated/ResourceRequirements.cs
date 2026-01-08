// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes the resource requirements for a container or a service.
    /// </summary>
    public partial class ResourceRequirements
    {
        /// <summary>
        /// Initializes a new instance of the ResourceRequirements class.
        /// </summary>
        /// <param name="requests">Describes the requested resources for a given container.</param>
        /// <param name="limits">Describes the maximum limits on the resources for a given container.</param>
        public ResourceRequirements(
            ResourceRequests requests,
            ResourceLimits limits = default(ResourceLimits))
        {
            requests.ThrowIfNull(nameof(requests));
            this.Requests = requests;
            this.Limits = limits;
        }

        /// <summary>
        /// Gets the requested resources for a given container.
        /// </summary>
        public ResourceRequests Requests { get; }

        /// <summary>
        /// Gets the maximum limits on the resources for a given container.
        /// </summary>
        public ResourceLimits Limits { get; }
    }
}
