// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for HealthEvaluationKind.
    /// </summary>
    public enum HealthEvaluationKind
    {
        /// <summary>
        /// Indicates that the health evaluation is invalid. The value is zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// Indicates that the health evaluation is for a health event. The value is 1.
        /// </summary>
        Event,

        /// <summary>
        /// Indicates that the health evaluation is for the replicas of a partition. The value is 2.
        /// </summary>
        Replicas,

        /// <summary>
        /// Indicates that the health evaluation is for the partitions of a service. The value is 3.
        /// </summary>
        Partitions,

        /// <summary>
        /// Indicates that the health evaluation is for the deployed service packages of a deployed application. The value is
        /// 4.
        /// </summary>
        DeployedServicePackages,

        /// <summary>
        /// Indicates that the health evaluation is for the deployed applications of an application. The value is 5.
        /// </summary>
        DeployedApplications,

        /// <summary>
        /// Indicates that the health evaluation is for services of an application. The value is 6.
        /// </summary>
        Services,

        /// <summary>
        /// Indicates that the health evaluation is for the cluster nodes. The value is 7.
        /// </summary>
        Nodes,

        /// <summary>
        /// Indicates that the health evaluation is for the cluster applications. The value is 8.
        /// </summary>
        Applications,

        /// <summary>
        /// Indicates that the health evaluation is for the system application. The value is 9.
        /// </summary>
        SystemApplication,

        /// <summary>
        /// Indicates that the health evaluation is for the deployed applications of an application in an upgrade domain. The
        /// value is 10.
        /// </summary>
        UpgradeDomainDeployedApplications,

        /// <summary>
        /// Indicates that the health evaluation is for the cluster nodes in an upgrade domain. The value is 11.
        /// </summary>
        UpgradeDomainNodes,

        /// <summary>
        /// Indicates that the health evaluation is for a replica. The value is 13.
        /// </summary>
        Replica,

        /// <summary>
        /// Indicates that the health evaluation is for a partition. The value is 14.
        /// </summary>
        Partition,

        /// <summary>
        /// Indicates that the health evaluation is for a deployed service package. The value is 16.
        /// </summary>
        DeployedServicePackage,

        /// <summary>
        /// Indicates that the health evaluation is for a deployed application. The value is 17.
        /// </summary>
        DeployedApplication,

        /// <summary>
        /// Indicates that the health evaluation is for a service. The value is 15.
        /// </summary>
        Service,

        /// <summary>
        /// Indicates that the health evaluation is for a node. The value is 12.
        /// </summary>
        Node,

        /// <summary>
        /// Indicates that the health evaluation is for an application. The value is 18.
        /// </summary>
        Application,

        /// <summary>
        /// Indicates that the health evaluation is for the delta of unhealthy cluster nodes. The value is 19.
        /// </summary>
        DeltaNodesCheck,

        /// <summary>
        /// Indicates that the health evaluation is for the delta of unhealthy upgrade domain cluster nodes. The value is 20.
        /// </summary>
        UpgradeDomainDeltaNodesCheck,

        /// <summary>
        /// – Indicates that the health evaluation is for applications of an application type. The value is 21.
        /// </summary>
        ApplicationTypeApplications,

        /// <summary>
        /// – Indicates that the health evaluation is for nodes of a node type. The value is 22.
        /// </summary>
        NodeTypeNodes,
    }
}
