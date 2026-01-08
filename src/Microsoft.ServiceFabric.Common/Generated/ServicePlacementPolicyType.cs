// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ServicePlacementPolicyType.
    /// </summary>
    public enum ServicePlacementPolicyType
    {
        /// <summary>
        /// Indicates the type of the placement policy is invalid. All Service Fabric enumerations have the invalid type. The
        /// value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the ServicePlacementPolicyDescription is of type ServicePlacementInvalidDomainPolicyDescription,
        /// which indicates that a particular fault or upgrade domain cannot be used for placement of this service. The value
        /// is 1.
        /// </summary>
        InvalidDomain,

        /// <summary>
        /// Indicates that the ServicePlacementPolicyDescription is of type
        /// ServicePlacementRequireDomainDistributionPolicyDescription indicating that the replicas of the service must be
        /// placed in a specific domain. The value is 2.
        /// </summary>
        RequireDomain,

        /// <summary>
        /// Indicates that the ServicePlacementPolicyDescription is of type
        /// ServicePlacementPreferPrimaryDomainPolicyDescription, which indicates that if possible the Primary replica for the
        /// partitions of the service should be located in a particular domain as an optimization. The value is 3.
        /// </summary>
        PreferPrimaryDomain,

        /// <summary>
        /// Indicates that the ServicePlacementPolicyDescription is of type
        /// ServicePlacementRequireDomainDistributionPolicyDescription, indicating that the system will disallow placement of
        /// any two replicas from the same partition in the same domain at any time. The value is 4.
        /// </summary>
        RequireDomainDistribution,

        /// <summary>
        /// Indicates that the ServicePlacementPolicyDescription is of type
        /// ServicePlacementNonPartiallyPlaceServicePolicyDescription, which indicates that if possible all replicas of a
        /// particular partition of the service should be placed atomically. The value is 5.
        /// </summary>
        NonPartiallyPlaceService,

        /// <summary>
        /// Indicates that the ServicePlacementPolicyDescription is of type
        /// ServicePlacementAllowMultipleStatelessInstancesOnNodePolicyDescription, which indicates that multiple stateless
        /// instances of a particular partition of the service can be placed on a node. The value is 6.
        /// </summary>
        AllowMultipleStatelessInstancesOnNode,
    }
}
