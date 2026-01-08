// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the policy to be used for placement of a Service Fabric service.
    /// </summary>
    public abstract partial class ServicePlacementPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServicePlacementPolicyDescription class.
        /// </summary>
        /// <param name="type">The type of placement policy for a service fabric service. Following are the possible
        /// values.</param>
        protected ServicePlacementPolicyDescription(
            ServicePlacementPolicyType? type)
        {
            type.ThrowIfNull(nameof(type));
            this.Type = type;
        }

        /// <summary>
        /// Gets the type of placement policy for a service fabric service. Following are the possible values.
        /// </summary>
        public ServicePlacementPolicyType? Type { get; }
    }
}
