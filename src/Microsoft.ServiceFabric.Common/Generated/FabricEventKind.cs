// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for FabricEventKind.
    /// </summary>
    public enum FabricEventKind
    {
        /// <summary>
        /// ClusterEvent.
        /// </summary>
        ClusterEvent,

        /// <summary>
        /// ContainerInstanceEvent.
        /// </summary>
        ContainerInstanceEvent,

        /// <summary>
        /// NodeEvent.
        /// </summary>
        NodeEvent,

        /// <summary>
        /// ApplicationEvent.
        /// </summary>
        ApplicationEvent,

        /// <summary>
        /// ServiceEvent.
        /// </summary>
        ServiceEvent,

        /// <summary>
        /// PartitionEvent.
        /// </summary>
        PartitionEvent,

        /// <summary>
        /// ReplicaEvent.
        /// </summary>
        ReplicaEvent,

        /// <summary>
        /// PartitionAnalysisEvent.
        /// </summary>
        PartitionAnalysisEvent,

        /// <summary>
        /// ApplicationCreated.
        /// </summary>
        ApplicationCreated,

        /// <summary>
        /// ApplicationDeleted.
        /// </summary>
        ApplicationDeleted,

        /// <summary>
        /// ApplicationNewHealthReport.
        /// </summary>
        ApplicationNewHealthReport,

        /// <summary>
        /// ApplicationHealthReportExpired.
        /// </summary>
        ApplicationHealthReportExpired,

        /// <summary>
        /// ApplicationUpgradeCompleted.
        /// </summary>
        ApplicationUpgradeCompleted,

        /// <summary>
        /// ApplicationUpgradeDomainCompleted.
        /// </summary>
        ApplicationUpgradeDomainCompleted,

        /// <summary>
        /// ApplicationUpgradeRollbackCompleted.
        /// </summary>
        ApplicationUpgradeRollbackCompleted,

        /// <summary>
        /// ApplicationUpgradeRollbackStarted.
        /// </summary>
        ApplicationUpgradeRollbackStarted,

        /// <summary>
        /// ApplicationUpgradeStarted.
        /// </summary>
        ApplicationUpgradeStarted,

        /// <summary>
        /// DeployedApplicationNewHealthReport.
        /// </summary>
        DeployedApplicationNewHealthReport,

        /// <summary>
        /// DeployedApplicationHealthReportExpired.
        /// </summary>
        DeployedApplicationHealthReportExpired,

        /// <summary>
        /// ApplicationProcessExited.
        /// </summary>
        ApplicationProcessExited,

        /// <summary>
        /// ApplicationContainerInstanceExited.
        /// </summary>
        ApplicationContainerInstanceExited,

        /// <summary>
        /// NodeAborted.
        /// </summary>
        NodeAborted,

        /// <summary>
        /// NodeAddedToCluster.
        /// </summary>
        NodeAddedToCluster,

        /// <summary>
        /// NodeClosed.
        /// </summary>
        NodeClosed,

        /// <summary>
        /// NodeDeactivateCompleted.
        /// </summary>
        NodeDeactivateCompleted,

        /// <summary>
        /// NodeDeactivateStarted.
        /// </summary>
        NodeDeactivateStarted,

        /// <summary>
        /// NodeDown.
        /// </summary>
        NodeDown,

        /// <summary>
        /// NodeNewHealthReport.
        /// </summary>
        NodeNewHealthReport,

        /// <summary>
        /// NodeHealthReportExpired.
        /// </summary>
        NodeHealthReportExpired,

        /// <summary>
        /// NodeOpenSucceeded.
        /// </summary>
        NodeOpenSucceeded,

        /// <summary>
        /// NodeOpenFailed.
        /// </summary>
        NodeOpenFailed,

        /// <summary>
        /// NodeRemovedFromCluster.
        /// </summary>
        NodeRemovedFromCluster,

        /// <summary>
        /// NodeUp.
        /// </summary>
        NodeUp,

        /// <summary>
        /// PartitionNewHealthReport.
        /// </summary>
        PartitionNewHealthReport,

        /// <summary>
        /// PartitionHealthReportExpired.
        /// </summary>
        PartitionHealthReportExpired,

        /// <summary>
        /// PartitionReconfigured.
        /// </summary>
        PartitionReconfigured,

        /// <summary>
        /// PartitionPrimaryMoveAnalysis.
        /// </summary>
        PartitionPrimaryMoveAnalysis,

        /// <summary>
        /// ServiceCreated.
        /// </summary>
        ServiceCreated,

        /// <summary>
        /// ServiceDeleted.
        /// </summary>
        ServiceDeleted,

        /// <summary>
        /// ServiceNewHealthReport.
        /// </summary>
        ServiceNewHealthReport,

        /// <summary>
        /// ServiceHealthReportExpired.
        /// </summary>
        ServiceHealthReportExpired,

        /// <summary>
        /// DeployedServicePackageNewHealthReport.
        /// </summary>
        DeployedServicePackageNewHealthReport,

        /// <summary>
        /// DeployedServicePackageHealthReportExpired.
        /// </summary>
        DeployedServicePackageHealthReportExpired,

        /// <summary>
        /// StatefulReplicaNewHealthReport.
        /// </summary>
        StatefulReplicaNewHealthReport,

        /// <summary>
        /// StatefulReplicaHealthReportExpired.
        /// </summary>
        StatefulReplicaHealthReportExpired,

        /// <summary>
        /// StatelessReplicaNewHealthReport.
        /// </summary>
        StatelessReplicaNewHealthReport,

        /// <summary>
        /// StatelessReplicaHealthReportExpired.
        /// </summary>
        StatelessReplicaHealthReportExpired,

        /// <summary>
        /// ClusterNewHealthReport.
        /// </summary>
        ClusterNewHealthReport,

        /// <summary>
        /// ClusterHealthReportExpired.
        /// </summary>
        ClusterHealthReportExpired,

        /// <summary>
        /// ClusterUpgradeCompleted.
        /// </summary>
        ClusterUpgradeCompleted,

        /// <summary>
        /// ClusterUpgradeDomainCompleted.
        /// </summary>
        ClusterUpgradeDomainCompleted,

        /// <summary>
        /// ClusterUpgradesNodesComplete.
        /// </summary>
        ClusterUpgradesNodesComplete,

        /// <summary>
        /// ClusterUpgradeRollbackCompleted.
        /// </summary>
        ClusterUpgradeRollbackCompleted,

        /// <summary>
        /// ClusterUpgradeRollbackStarted.
        /// </summary>
        ClusterUpgradeRollbackStarted,

        /// <summary>
        /// ClusterUpgradeStarted.
        /// </summary>
        ClusterUpgradeStarted,

        /// <summary>
        /// ChaosStopped.
        /// </summary>
        ChaosStopped,

        /// <summary>
        /// ChaosStarted.
        /// </summary>
        ChaosStarted,

        /// <summary>
        /// ChaosCodePackageRestartScheduled.
        /// </summary>
        ChaosCodePackageRestartScheduled,

        /// <summary>
        /// ChaosReplicaRemovalScheduled.
        /// </summary>
        ChaosReplicaRemovalScheduled,

        /// <summary>
        /// ChaosPartitionSecondaryMoveScheduled.
        /// </summary>
        ChaosPartitionSecondaryMoveScheduled,

        /// <summary>
        /// ChaosPartitionPrimaryMoveScheduled.
        /// </summary>
        ChaosPartitionPrimaryMoveScheduled,

        /// <summary>
        /// ChaosReplicaRestartScheduled.
        /// </summary>
        ChaosReplicaRestartScheduled,

        /// <summary>
        /// ChaosNodeRestartScheduled.
        /// </summary>
        ChaosNodeRestartScheduled,

        /// <summary>
        /// ReadinessProbeFailed.
        /// </summary>
        ReadinessProbeFailed,

        /// <summary>
        /// LivenessProbeFailed.
        /// </summary>
        LivenessProbeFailed,

        /// <summary>
        /// Decision.
        /// </summary>
        Decision,

        /// <summary>
        /// BRSWarningCluster.
        /// </summary>
        BRSWarningCluster,

        /// <summary>
        /// BRSInfoCluster.
        /// </summary>
        BRSInfoCluster,

        /// <summary>
        /// BRSErrorCluster.
        /// </summary>
        BRSErrorCluster,

        /// <summary>
        /// BRSWarningPartition.
        /// </summary>
        BRSWarningPartition,

        /// <summary>
        /// BRSInfoPartition.
        /// </summary>
        BRSInfoPartition,

        /// <summary>
        /// BRSErrorPartition.
        /// </summary>
        BRSErrorPartition,
    }
}
