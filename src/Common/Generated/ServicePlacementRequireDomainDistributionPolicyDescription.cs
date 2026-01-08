// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the policy to be used for placement of a Service Fabric service where two replicas from the same
    /// partition should never be placed in the same fault or upgrade domain.
    /// 
    /// While this is not common it can expose the service to an increased risk of concurrent failures due to unplanned
    /// outages or other cases of subsequent/concurrent failures. As an example, consider a case where replicas are
    /// deployed across different data center, with one replica per location. In the event that one of the datacenters goes
    /// offline, normally the replica that was placed in that datacenter will be packed into one of the remaining
    /// datacenters. If this is not desirable then this policy should be set.
    /// </summary>
    public partial class ServicePlacementRequireDomainDistributionPolicyDescription : ServicePlacementPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServicePlacementRequireDomainDistributionPolicyDescription class.
        /// </summary>
        /// <param name="domainName">The name of the domain that should used for placement as per this policy.</param>
        public ServicePlacementRequireDomainDistributionPolicyDescription(
            string domainName = default(string))
            : base(
                Common.ServicePlacementPolicyType.RequireDomainDistribution)
        {
            this.DomainName = domainName;
        }

        /// <summary>
        /// Gets the name of the domain that should used for placement as per this policy.
        /// </summary>
        public string DomainName { get; }
    }
}
