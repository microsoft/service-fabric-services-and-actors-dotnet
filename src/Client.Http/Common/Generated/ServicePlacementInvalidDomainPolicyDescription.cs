// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the policy to be used for placement of a Service Fabric service where a particular fault or upgrade
    /// domain should not be used for placement of the instances or replicas of that service.
    /// </summary>
    public partial class ServicePlacementInvalidDomainPolicyDescription : ServicePlacementPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServicePlacementInvalidDomainPolicyDescription class.
        /// </summary>
        /// <param name="domainName">The name of the domain that should not be used for placement.</param>
        public ServicePlacementInvalidDomainPolicyDescription(
            string domainName = default(string))
            : base(
                Common.ServicePlacementPolicyType.InvalidDomain)
        {
            this.DomainName = domainName;
        }

        /// <summary>
        /// Gets the name of the domain that should not be used for placement.
        /// </summary>
        public string DomainName { get; }
    }
}
