// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the policy to be used for placement of a Service Fabric service allowing multiple stateless instances of
    /// a partition of the service to be placed on a node.
    /// </summary>
    public partial class ServicePlacementAllowMultipleStatelessInstancesOnNodePolicyDescription : ServicePlacementPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServicePlacementAllowMultipleStatelessInstancesOnNodePolicyDescription class.
        /// </summary>
        /// <param name="domainName">Holdover from other policy descriptions, not used for this policy, values are ignored by
        /// runtime. Keeping it for any backwards-compatibility with clients.</param>
        public ServicePlacementAllowMultipleStatelessInstancesOnNodePolicyDescription(
            string domainName = default(string))
            : base(
                Common.ServicePlacementPolicyType.AllowMultipleStatelessInstancesOnNode)
        {
            this.DomainName = domainName;
        }

        /// <summary>
        /// Gets holdover from other policy descriptions, not used for this policy, values are ignored by runtime. Keeping it
        /// for any backwards-compatibility with clients.
        /// </summary>
        public string DomainName { get; }
    }
}
