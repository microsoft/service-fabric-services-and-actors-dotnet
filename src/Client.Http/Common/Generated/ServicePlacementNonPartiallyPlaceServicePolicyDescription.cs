// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the policy to be used for placement of a Service Fabric service where all replicas must be able to be
    /// placed in order for any replicas to be created.
    /// </summary>
    public partial class ServicePlacementNonPartiallyPlaceServicePolicyDescription : ServicePlacementPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServicePlacementNonPartiallyPlaceServicePolicyDescription class.
        /// </summary>
        public ServicePlacementNonPartiallyPlaceServicePolicyDescription()
            : base(
                Common.ServicePlacementPolicyType.NonPartiallyPlaceService)
        {
        }
    }
}
