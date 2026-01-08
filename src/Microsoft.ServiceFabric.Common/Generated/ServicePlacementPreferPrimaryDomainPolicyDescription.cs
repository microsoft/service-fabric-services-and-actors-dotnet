// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the policy to be used for placement of a Service Fabric service where the service's Primary replicas
    /// should optimally be placed in a particular domain.
    /// 
    /// This placement policy is usually used with fault domains in scenarios where the Service Fabric cluster is
    /// geographically distributed in order to indicate that a service's primary replica should be located in a particular
    /// fault domain, which in geo-distributed scenarios usually aligns with regional or datacenter boundaries. Note that
    /// since this is an optimization it is possible that the Primary replica may not end up located in this domain due to
    /// failures, capacity limits, or other constraints.
    /// </summary>
    public partial class ServicePlacementPreferPrimaryDomainPolicyDescription : ServicePlacementPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServicePlacementPreferPrimaryDomainPolicyDescription class.
        /// </summary>
        /// <param name="domainName">The name of the domain that should used for placement as per this policy.</param>
        public ServicePlacementPreferPrimaryDomainPolicyDescription(
            string domainName = default(string))
            : base(
                Common.ServicePlacementPolicyType.PreferPrimaryDomain)
        {
            this.DomainName = domainName;
        }

        /// <summary>
        /// Gets the name of the domain that should used for placement as per this policy.
        /// </summary>
        public string DomainName { get; }
    }
}
