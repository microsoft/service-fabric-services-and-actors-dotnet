// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the policy to be used for placement of a Service Fabric service where the instances or replicas of that
    /// service must be placed in a particular domain
    /// </summary>
    public partial class ServicePlacementRequiredDomainPolicyDescription : ServicePlacementPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServicePlacementRequiredDomainPolicyDescription class.
        /// </summary>
        /// <param name="domainName">The name of the domain that should used for placement as per this policy.</param>
        public ServicePlacementRequiredDomainPolicyDescription(
            string domainName = default(string))
            : base(
                Common.ServicePlacementPolicyType.RequireDomain)
        {
            this.DomainName = domainName;
        }

        /// <summary>
        /// Gets the name of the domain that should used for placement as per this policy.
        /// </summary>
        public string DomainName { get; }
    }
}
