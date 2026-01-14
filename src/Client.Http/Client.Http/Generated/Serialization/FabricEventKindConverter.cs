// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Converter for <see cref="FabricEventKind" />.
    /// </summary>
    internal class FabricEventKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static FabricEventKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(FabricEventKind);

            if (string.Compare(value, "ClusterEvent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterEvent;
            }
            else if (string.Compare(value, "ContainerInstanceEvent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ContainerInstanceEvent;
            }
            else if (string.Compare(value, "NodeEvent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeEvent;
            }
            else if (string.Compare(value, "ApplicationEvent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationEvent;
            }
            else if (string.Compare(value, "ServiceEvent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ServiceEvent;
            }
            else if (string.Compare(value, "PartitionEvent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.PartitionEvent;
            }
            else if (string.Compare(value, "ReplicaEvent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ReplicaEvent;
            }
            else if (string.Compare(value, "PartitionAnalysisEvent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.PartitionAnalysisEvent;
            }
            else if (string.Compare(value, "ApplicationCreated", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationCreated;
            }
            else if (string.Compare(value, "ApplicationDeleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationDeleted;
            }
            else if (string.Compare(value, "ApplicationNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationNewHealthReport;
            }
            else if (string.Compare(value, "ApplicationHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationHealthReportExpired;
            }
            else if (string.Compare(value, "ApplicationUpgradeCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationUpgradeCompleted;
            }
            else if (string.Compare(value, "ApplicationUpgradeDomainCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationUpgradeDomainCompleted;
            }
            else if (string.Compare(value, "ApplicationUpgradeRollbackCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationUpgradeRollbackCompleted;
            }
            else if (string.Compare(value, "ApplicationUpgradeRollbackStarted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationUpgradeRollbackStarted;
            }
            else if (string.Compare(value, "ApplicationUpgradeStarted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationUpgradeStarted;
            }
            else if (string.Compare(value, "DeployedApplicationNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.DeployedApplicationNewHealthReport;
            }
            else if (string.Compare(value, "DeployedApplicationHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.DeployedApplicationHealthReportExpired;
            }
            else if (string.Compare(value, "ApplicationProcessExited", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationProcessExited;
            }
            else if (string.Compare(value, "ApplicationContainerInstanceExited", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ApplicationContainerInstanceExited;
            }
            else if (string.Compare(value, "NodeAborted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeAborted;
            }
            else if (string.Compare(value, "NodeAddedToCluster", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeAddedToCluster;
            }
            else if (string.Compare(value, "NodeClosed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeClosed;
            }
            else if (string.Compare(value, "NodeDeactivateCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeDeactivateCompleted;
            }
            else if (string.Compare(value, "NodeDeactivateStarted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeDeactivateStarted;
            }
            else if (string.Compare(value, "NodeDown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeDown;
            }
            else if (string.Compare(value, "NodeNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeNewHealthReport;
            }
            else if (string.Compare(value, "NodeHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeHealthReportExpired;
            }
            else if (string.Compare(value, "NodeOpenSucceeded", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeOpenSucceeded;
            }
            else if (string.Compare(value, "NodeOpenFailed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeOpenFailed;
            }
            else if (string.Compare(value, "NodeRemovedFromCluster", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeRemovedFromCluster;
            }
            else if (string.Compare(value, "NodeUp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.NodeUp;
            }
            else if (string.Compare(value, "PartitionNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.PartitionNewHealthReport;
            }
            else if (string.Compare(value, "PartitionHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.PartitionHealthReportExpired;
            }
            else if (string.Compare(value, "PartitionReconfigured", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.PartitionReconfigured;
            }
            else if (string.Compare(value, "PartitionPrimaryMoveAnalysis", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.PartitionPrimaryMoveAnalysis;
            }
            else if (string.Compare(value, "ServiceCreated", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ServiceCreated;
            }
            else if (string.Compare(value, "ServiceDeleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ServiceDeleted;
            }
            else if (string.Compare(value, "ServiceNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ServiceNewHealthReport;
            }
            else if (string.Compare(value, "ServiceHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ServiceHealthReportExpired;
            }
            else if (string.Compare(value, "DeployedServicePackageNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.DeployedServicePackageNewHealthReport;
            }
            else if (string.Compare(value, "DeployedServicePackageHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.DeployedServicePackageHealthReportExpired;
            }
            else if (string.Compare(value, "StatefulReplicaNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.StatefulReplicaNewHealthReport;
            }
            else if (string.Compare(value, "StatefulReplicaHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.StatefulReplicaHealthReportExpired;
            }
            else if (string.Compare(value, "StatelessReplicaNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.StatelessReplicaNewHealthReport;
            }
            else if (string.Compare(value, "StatelessReplicaHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.StatelessReplicaHealthReportExpired;
            }
            else if (string.Compare(value, "ClusterNewHealthReport", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterNewHealthReport;
            }
            else if (string.Compare(value, "ClusterHealthReportExpired", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterHealthReportExpired;
            }
            else if (string.Compare(value, "ClusterUpgradeCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterUpgradeCompleted;
            }
            else if (string.Compare(value, "ClusterUpgradeDomainCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterUpgradeDomainCompleted;
            }
            else if (string.Compare(value, "ClusterUpgradesNodesComplete", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterUpgradesNodesComplete;
            }
            else if (string.Compare(value, "ClusterUpgradeRollbackCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterUpgradeRollbackCompleted;
            }
            else if (string.Compare(value, "ClusterUpgradeRollbackStarted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterUpgradeRollbackStarted;
            }
            else if (string.Compare(value, "ClusterUpgradeStarted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ClusterUpgradeStarted;
            }
            else if (string.Compare(value, "ChaosStopped", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ChaosStopped;
            }
            else if (string.Compare(value, "ChaosStarted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ChaosStarted;
            }
            else if (string.Compare(value, "ChaosCodePackageRestartScheduled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ChaosCodePackageRestartScheduled;
            }
            else if (string.Compare(value, "ChaosReplicaRemovalScheduled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ChaosReplicaRemovalScheduled;
            }
            else if (string.Compare(value, "ChaosPartitionSecondaryMoveScheduled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ChaosPartitionSecondaryMoveScheduled;
            }
            else if (string.Compare(value, "ChaosPartitionPrimaryMoveScheduled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ChaosPartitionPrimaryMoveScheduled;
            }
            else if (string.Compare(value, "ChaosReplicaRestartScheduled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ChaosReplicaRestartScheduled;
            }
            else if (string.Compare(value, "ChaosNodeRestartScheduled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ChaosNodeRestartScheduled;
            }
            else if (string.Compare(value, "ReadinessProbeFailed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.ReadinessProbeFailed;
            }
            else if (string.Compare(value, "LivenessProbeFailed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.LivenessProbeFailed;
            }
            else if (string.Compare(value, "Decision", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.Decision;
            }
            else if (string.Compare(value, "BRSWarningCluster", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.BRSWarningCluster;
            }
            else if (string.Compare(value, "BRSInfoCluster", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.BRSInfoCluster;
            }
            else if (string.Compare(value, "BRSErrorCluster", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.BRSErrorCluster;
            }
            else if (string.Compare(value, "BRSWarningPartition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.BRSWarningPartition;
            }
            else if (string.Compare(value, "BRSInfoPartition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.BRSInfoPartition;
            }
            else if (string.Compare(value, "BRSErrorPartition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricEventKind.BRSErrorPartition;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, FabricEventKind? value)
        {
            switch (value)
            {
                case FabricEventKind.ClusterEvent:
                    writer.WriteStringValue("ClusterEvent");
                    break;
                case FabricEventKind.ContainerInstanceEvent:
                    writer.WriteStringValue("ContainerInstanceEvent");
                    break;
                case FabricEventKind.NodeEvent:
                    writer.WriteStringValue("NodeEvent");
                    break;
                case FabricEventKind.ApplicationEvent:
                    writer.WriteStringValue("ApplicationEvent");
                    break;
                case FabricEventKind.ServiceEvent:
                    writer.WriteStringValue("ServiceEvent");
                    break;
                case FabricEventKind.PartitionEvent:
                    writer.WriteStringValue("PartitionEvent");
                    break;
                case FabricEventKind.ReplicaEvent:
                    writer.WriteStringValue("ReplicaEvent");
                    break;
                case FabricEventKind.PartitionAnalysisEvent:
                    writer.WriteStringValue("PartitionAnalysisEvent");
                    break;
                case FabricEventKind.ApplicationCreated:
                    writer.WriteStringValue("ApplicationCreated");
                    break;
                case FabricEventKind.ApplicationDeleted:
                    writer.WriteStringValue("ApplicationDeleted");
                    break;
                case FabricEventKind.ApplicationNewHealthReport:
                    writer.WriteStringValue("ApplicationNewHealthReport");
                    break;
                case FabricEventKind.ApplicationHealthReportExpired:
                    writer.WriteStringValue("ApplicationHealthReportExpired");
                    break;
                case FabricEventKind.ApplicationUpgradeCompleted:
                    writer.WriteStringValue("ApplicationUpgradeCompleted");
                    break;
                case FabricEventKind.ApplicationUpgradeDomainCompleted:
                    writer.WriteStringValue("ApplicationUpgradeDomainCompleted");
                    break;
                case FabricEventKind.ApplicationUpgradeRollbackCompleted:
                    writer.WriteStringValue("ApplicationUpgradeRollbackCompleted");
                    break;
                case FabricEventKind.ApplicationUpgradeRollbackStarted:
                    writer.WriteStringValue("ApplicationUpgradeRollbackStarted");
                    break;
                case FabricEventKind.ApplicationUpgradeStarted:
                    writer.WriteStringValue("ApplicationUpgradeStarted");
                    break;
                case FabricEventKind.DeployedApplicationNewHealthReport:
                    writer.WriteStringValue("DeployedApplicationNewHealthReport");
                    break;
                case FabricEventKind.DeployedApplicationHealthReportExpired:
                    writer.WriteStringValue("DeployedApplicationHealthReportExpired");
                    break;
                case FabricEventKind.ApplicationProcessExited:
                    writer.WriteStringValue("ApplicationProcessExited");
                    break;
                case FabricEventKind.ApplicationContainerInstanceExited:
                    writer.WriteStringValue("ApplicationContainerInstanceExited");
                    break;
                case FabricEventKind.NodeAborted:
                    writer.WriteStringValue("NodeAborted");
                    break;
                case FabricEventKind.NodeAddedToCluster:
                    writer.WriteStringValue("NodeAddedToCluster");
                    break;
                case FabricEventKind.NodeClosed:
                    writer.WriteStringValue("NodeClosed");
                    break;
                case FabricEventKind.NodeDeactivateCompleted:
                    writer.WriteStringValue("NodeDeactivateCompleted");
                    break;
                case FabricEventKind.NodeDeactivateStarted:
                    writer.WriteStringValue("NodeDeactivateStarted");
                    break;
                case FabricEventKind.NodeDown:
                    writer.WriteStringValue("NodeDown");
                    break;
                case FabricEventKind.NodeNewHealthReport:
                    writer.WriteStringValue("NodeNewHealthReport");
                    break;
                case FabricEventKind.NodeHealthReportExpired:
                    writer.WriteStringValue("NodeHealthReportExpired");
                    break;
                case FabricEventKind.NodeOpenSucceeded:
                    writer.WriteStringValue("NodeOpenSucceeded");
                    break;
                case FabricEventKind.NodeOpenFailed:
                    writer.WriteStringValue("NodeOpenFailed");
                    break;
                case FabricEventKind.NodeRemovedFromCluster:
                    writer.WriteStringValue("NodeRemovedFromCluster");
                    break;
                case FabricEventKind.NodeUp:
                    writer.WriteStringValue("NodeUp");
                    break;
                case FabricEventKind.PartitionNewHealthReport:
                    writer.WriteStringValue("PartitionNewHealthReport");
                    break;
                case FabricEventKind.PartitionHealthReportExpired:
                    writer.WriteStringValue("PartitionHealthReportExpired");
                    break;
                case FabricEventKind.PartitionReconfigured:
                    writer.WriteStringValue("PartitionReconfigured");
                    break;
                case FabricEventKind.PartitionPrimaryMoveAnalysis:
                    writer.WriteStringValue("PartitionPrimaryMoveAnalysis");
                    break;
                case FabricEventKind.ServiceCreated:
                    writer.WriteStringValue("ServiceCreated");
                    break;
                case FabricEventKind.ServiceDeleted:
                    writer.WriteStringValue("ServiceDeleted");
                    break;
                case FabricEventKind.ServiceNewHealthReport:
                    writer.WriteStringValue("ServiceNewHealthReport");
                    break;
                case FabricEventKind.ServiceHealthReportExpired:
                    writer.WriteStringValue("ServiceHealthReportExpired");
                    break;
                case FabricEventKind.DeployedServicePackageNewHealthReport:
                    writer.WriteStringValue("DeployedServicePackageNewHealthReport");
                    break;
                case FabricEventKind.DeployedServicePackageHealthReportExpired:
                    writer.WriteStringValue("DeployedServicePackageHealthReportExpired");
                    break;
                case FabricEventKind.StatefulReplicaNewHealthReport:
                    writer.WriteStringValue("StatefulReplicaNewHealthReport");
                    break;
                case FabricEventKind.StatefulReplicaHealthReportExpired:
                    writer.WriteStringValue("StatefulReplicaHealthReportExpired");
                    break;
                case FabricEventKind.StatelessReplicaNewHealthReport:
                    writer.WriteStringValue("StatelessReplicaNewHealthReport");
                    break;
                case FabricEventKind.StatelessReplicaHealthReportExpired:
                    writer.WriteStringValue("StatelessReplicaHealthReportExpired");
                    break;
                case FabricEventKind.ClusterNewHealthReport:
                    writer.WriteStringValue("ClusterNewHealthReport");
                    break;
                case FabricEventKind.ClusterHealthReportExpired:
                    writer.WriteStringValue("ClusterHealthReportExpired");
                    break;
                case FabricEventKind.ClusterUpgradeCompleted:
                    writer.WriteStringValue("ClusterUpgradeCompleted");
                    break;
                case FabricEventKind.ClusterUpgradeDomainCompleted:
                    writer.WriteStringValue("ClusterUpgradeDomainCompleted");
                    break;
                case FabricEventKind.ClusterUpgradesNodesComplete:
                    writer.WriteStringValue("ClusterUpgradesNodesComplete");
                    break;
                case FabricEventKind.ClusterUpgradeRollbackCompleted:
                    writer.WriteStringValue("ClusterUpgradeRollbackCompleted");
                    break;
                case FabricEventKind.ClusterUpgradeRollbackStarted:
                    writer.WriteStringValue("ClusterUpgradeRollbackStarted");
                    break;
                case FabricEventKind.ClusterUpgradeStarted:
                    writer.WriteStringValue("ClusterUpgradeStarted");
                    break;
                case FabricEventKind.ChaosStopped:
                    writer.WriteStringValue("ChaosStopped");
                    break;
                case FabricEventKind.ChaosStarted:
                    writer.WriteStringValue("ChaosStarted");
                    break;
                case FabricEventKind.ChaosCodePackageRestartScheduled:
                    writer.WriteStringValue("ChaosCodePackageRestartScheduled");
                    break;
                case FabricEventKind.ChaosReplicaRemovalScheduled:
                    writer.WriteStringValue("ChaosReplicaRemovalScheduled");
                    break;
                case FabricEventKind.ChaosPartitionSecondaryMoveScheduled:
                    writer.WriteStringValue("ChaosPartitionSecondaryMoveScheduled");
                    break;
                case FabricEventKind.ChaosPartitionPrimaryMoveScheduled:
                    writer.WriteStringValue("ChaosPartitionPrimaryMoveScheduled");
                    break;
                case FabricEventKind.ChaosReplicaRestartScheduled:
                    writer.WriteStringValue("ChaosReplicaRestartScheduled");
                    break;
                case FabricEventKind.ChaosNodeRestartScheduled:
                    writer.WriteStringValue("ChaosNodeRestartScheduled");
                    break;
                case FabricEventKind.ReadinessProbeFailed:
                    writer.WriteStringValue("ReadinessProbeFailed");
                    break;
                case FabricEventKind.LivenessProbeFailed:
                    writer.WriteStringValue("LivenessProbeFailed");
                    break;
                case FabricEventKind.Decision:
                    writer.WriteStringValue("Decision");
                    break;
                case FabricEventKind.BRSWarningCluster:
                    writer.WriteStringValue("BRSWarningCluster");
                    break;
                case FabricEventKind.BRSInfoCluster:
                    writer.WriteStringValue("BRSInfoCluster");
                    break;
                case FabricEventKind.BRSErrorCluster:
                    writer.WriteStringValue("BRSErrorCluster");
                    break;
                case FabricEventKind.BRSWarningPartition:
                    writer.WriteStringValue("BRSWarningPartition");
                    break;
                case FabricEventKind.BRSInfoPartition:
                    writer.WriteStringValue("BRSInfoPartition");
                    break;
                case FabricEventKind.BRSErrorPartition:
                    writer.WriteStringValue("BRSErrorPartition");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type FabricEventKind");
            }
        }
    }
}
