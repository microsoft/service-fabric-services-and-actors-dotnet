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
    /// Converter for <see cref="FabricEvent" />.
    /// </summary>
    internal class FabricEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static FabricEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static FabricEvent GetFromJsonProperties(JsonReader reader)
        {
            FabricEvent obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Kind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("ApplicationEvent", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterEvent", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ContainerInstanceEvent", StringComparison.OrdinalIgnoreCase))
            {
                obj = ContainerInstanceEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeEvent", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("PartitionAnalysisEvent", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionAnalysisEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("PartitionEvent", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ReplicaEvent", StringComparison.OrdinalIgnoreCase))
            {
                obj = ReplicaEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ServiceEvent", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServiceEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationCreated", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationCreatedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationDeleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationDeletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationUpgradeCompleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationUpgradeCompletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationUpgradeDomainCompleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationUpgradeDomainCompletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationUpgradeRollbackCompleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationUpgradeRollbackCompletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationUpgradeRollbackStarted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationUpgradeRollbackStartedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationUpgradeStarted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationUpgradeStartedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeployedApplicationNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedApplicationNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeployedApplicationHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedApplicationHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationProcessExited", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationProcessExitedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationContainerInstanceExited", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationContainerInstanceExitedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeAborted", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeAbortedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeAddedToCluster", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeAddedToClusterEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeClosed", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeClosedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeDeactivateCompleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeDeactivateCompletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeDeactivateStarted", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeDeactivateStartedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeDown", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeDownEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeOpenSucceeded", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeOpenSucceededEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeOpenFailed", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeOpenFailedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeRemovedFromCluster", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeRemovedFromClusterEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeUp", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeUpEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("PartitionNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("PartitionHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("PartitionReconfigured", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionReconfiguredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("PartitionPrimaryMoveAnalysis", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionPrimaryMoveAnalysisEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ServiceCreated", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServiceCreatedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ServiceDeleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServiceDeletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ServiceNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServiceNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ServiceHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServiceHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeployedServicePackageNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedServicePackageNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeployedServicePackageHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedServicePackageHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("StatefulReplicaNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = StatefulReplicaNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("StatefulReplicaHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = StatefulReplicaHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("StatelessReplicaNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = StatelessReplicaNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("StatelessReplicaHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = StatelessReplicaHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterNewHealthReport", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterNewHealthReportEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterHealthReportExpired", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterUpgradeCompleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterUpgradeCompletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterUpgradeDomainCompleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterUpgradeDomainCompletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterUpgradeRollbackCompleted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterUpgradeRollbackCompletedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterUpgradeRollbackStarted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterUpgradeRollbackStartedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterUpgradeStarted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterUpgradeStartedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ChaosStopped", StringComparison.OrdinalIgnoreCase))
            {
                obj = ChaosStoppedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ChaosStarted", StringComparison.OrdinalIgnoreCase))
            {
                obj = ChaosStartedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ChaosCodePackageRestartScheduled", StringComparison.OrdinalIgnoreCase))
            {
                obj = ChaosCodePackageRestartScheduledEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ChaosReplicaRemovalScheduled", StringComparison.OrdinalIgnoreCase))
            {
                obj = ChaosReplicaRemovalScheduledEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ChaosPartitionSecondaryMoveScheduled", StringComparison.OrdinalIgnoreCase))
            {
                obj = ChaosPartitionSecondaryMoveScheduledEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ChaosPartitionPrimaryMoveScheduled", StringComparison.OrdinalIgnoreCase))
            {
                obj = ChaosPartitionPrimaryMoveScheduledEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ChaosReplicaRestartScheduled", StringComparison.OrdinalIgnoreCase))
            {
                obj = ChaosReplicaRestartScheduledEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ChaosNodeRestartScheduled", StringComparison.OrdinalIgnoreCase))
            {
                obj = ChaosNodeRestartScheduledEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ReadinessProbeFailed", StringComparison.OrdinalIgnoreCase))
            {
                obj = ReadinessProbeFailedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("LivenessProbeFailed", StringComparison.OrdinalIgnoreCase))
            {
                obj = LivenessProbeFailedEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Decision", StringComparison.OrdinalIgnoreCase))
            {
                obj = DecisionEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ClusterUpgradesNodesComplete", StringComparison.OrdinalIgnoreCase))
            {
                obj = ClusterUpgradesNodesCompleteEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("BRSWarningCluster", StringComparison.OrdinalIgnoreCase))
            {
                obj = BRSWarningClusterEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("BRSInfoCluster", StringComparison.OrdinalIgnoreCase))
            {
                obj = BRSInfoClusterEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("BRSErrorCluster", StringComparison.OrdinalIgnoreCase))
            {
                obj = BRSErrorClusterEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("BRSWarningPartition", StringComparison.OrdinalIgnoreCase))
            {
                obj = BRSWarningPartitionEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("BRSInfoPartition", StringComparison.OrdinalIgnoreCase))
            {
                obj = BRSInfoPartitionEventConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("BRSErrorPartition", StringComparison.OrdinalIgnoreCase))
            {
                obj = BRSErrorPartitionEventConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, FabricEvent obj)
        {
            var kind = obj.Kind;
            if (kind.Equals(FabricEventKind.ApplicationEvent))
            {
                ApplicationEventConverter.Serialize(writer, (ApplicationEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterEvent))
            {
                ClusterEventConverter.Serialize(writer, (ClusterEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ContainerInstanceEvent))
            {
                ContainerInstanceEventConverter.Serialize(writer, (ContainerInstanceEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeEvent))
            {
                NodeEventConverter.Serialize(writer, (NodeEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.PartitionAnalysisEvent))
            {
                PartitionAnalysisEventConverter.Serialize(writer, (PartitionAnalysisEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.PartitionEvent))
            {
                PartitionEventConverter.Serialize(writer, (PartitionEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ReplicaEvent))
            {
                ReplicaEventConverter.Serialize(writer, (ReplicaEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ServiceEvent))
            {
                ServiceEventConverter.Serialize(writer, (ServiceEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationCreated))
            {
                ApplicationCreatedEventConverter.Serialize(writer, (ApplicationCreatedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationDeleted))
            {
                ApplicationDeletedEventConverter.Serialize(writer, (ApplicationDeletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationNewHealthReport))
            {
                ApplicationNewHealthReportEventConverter.Serialize(writer, (ApplicationNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationHealthReportExpired))
            {
                ApplicationHealthReportExpiredEventConverter.Serialize(writer, (ApplicationHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationUpgradeCompleted))
            {
                ApplicationUpgradeCompletedEventConverter.Serialize(writer, (ApplicationUpgradeCompletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationUpgradeDomainCompleted))
            {
                ApplicationUpgradeDomainCompletedEventConverter.Serialize(writer, (ApplicationUpgradeDomainCompletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationUpgradeRollbackCompleted))
            {
                ApplicationUpgradeRollbackCompletedEventConverter.Serialize(writer, (ApplicationUpgradeRollbackCompletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationUpgradeRollbackStarted))
            {
                ApplicationUpgradeRollbackStartedEventConverter.Serialize(writer, (ApplicationUpgradeRollbackStartedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationUpgradeStarted))
            {
                ApplicationUpgradeStartedEventConverter.Serialize(writer, (ApplicationUpgradeStartedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.DeployedApplicationNewHealthReport))
            {
                DeployedApplicationNewHealthReportEventConverter.Serialize(writer, (DeployedApplicationNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.DeployedApplicationHealthReportExpired))
            {
                DeployedApplicationHealthReportExpiredEventConverter.Serialize(writer, (DeployedApplicationHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationProcessExited))
            {
                ApplicationProcessExitedEventConverter.Serialize(writer, (ApplicationProcessExitedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ApplicationContainerInstanceExited))
            {
                ApplicationContainerInstanceExitedEventConverter.Serialize(writer, (ApplicationContainerInstanceExitedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeAborted))
            {
                NodeAbortedEventConverter.Serialize(writer, (NodeAbortedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeAddedToCluster))
            {
                NodeAddedToClusterEventConverter.Serialize(writer, (NodeAddedToClusterEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeClosed))
            {
                NodeClosedEventConverter.Serialize(writer, (NodeClosedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeDeactivateCompleted))
            {
                NodeDeactivateCompletedEventConverter.Serialize(writer, (NodeDeactivateCompletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeDeactivateStarted))
            {
                NodeDeactivateStartedEventConverter.Serialize(writer, (NodeDeactivateStartedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeDown))
            {
                NodeDownEventConverter.Serialize(writer, (NodeDownEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeNewHealthReport))
            {
                NodeNewHealthReportEventConverter.Serialize(writer, (NodeNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeHealthReportExpired))
            {
                NodeHealthReportExpiredEventConverter.Serialize(writer, (NodeHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeOpenSucceeded))
            {
                NodeOpenSucceededEventConverter.Serialize(writer, (NodeOpenSucceededEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeOpenFailed))
            {
                NodeOpenFailedEventConverter.Serialize(writer, (NodeOpenFailedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeRemovedFromCluster))
            {
                NodeRemovedFromClusterEventConverter.Serialize(writer, (NodeRemovedFromClusterEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.NodeUp))
            {
                NodeUpEventConverter.Serialize(writer, (NodeUpEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.PartitionNewHealthReport))
            {
                PartitionNewHealthReportEventConverter.Serialize(writer, (PartitionNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.PartitionHealthReportExpired))
            {
                PartitionHealthReportExpiredEventConverter.Serialize(writer, (PartitionHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.PartitionReconfigured))
            {
                PartitionReconfiguredEventConverter.Serialize(writer, (PartitionReconfiguredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.PartitionPrimaryMoveAnalysis))
            {
                PartitionPrimaryMoveAnalysisEventConverter.Serialize(writer, (PartitionPrimaryMoveAnalysisEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ServiceCreated))
            {
                ServiceCreatedEventConverter.Serialize(writer, (ServiceCreatedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ServiceDeleted))
            {
                ServiceDeletedEventConverter.Serialize(writer, (ServiceDeletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ServiceNewHealthReport))
            {
                ServiceNewHealthReportEventConverter.Serialize(writer, (ServiceNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ServiceHealthReportExpired))
            {
                ServiceHealthReportExpiredEventConverter.Serialize(writer, (ServiceHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.DeployedServicePackageNewHealthReport))
            {
                DeployedServicePackageNewHealthReportEventConverter.Serialize(writer, (DeployedServicePackageNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.DeployedServicePackageHealthReportExpired))
            {
                DeployedServicePackageHealthReportExpiredEventConverter.Serialize(writer, (DeployedServicePackageHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.StatefulReplicaNewHealthReport))
            {
                StatefulReplicaNewHealthReportEventConverter.Serialize(writer, (StatefulReplicaNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.StatefulReplicaHealthReportExpired))
            {
                StatefulReplicaHealthReportExpiredEventConverter.Serialize(writer, (StatefulReplicaHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.StatelessReplicaNewHealthReport))
            {
                StatelessReplicaNewHealthReportEventConverter.Serialize(writer, (StatelessReplicaNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.StatelessReplicaHealthReportExpired))
            {
                StatelessReplicaHealthReportExpiredEventConverter.Serialize(writer, (StatelessReplicaHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterNewHealthReport))
            {
                ClusterNewHealthReportEventConverter.Serialize(writer, (ClusterNewHealthReportEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterHealthReportExpired))
            {
                ClusterHealthReportExpiredEventConverter.Serialize(writer, (ClusterHealthReportExpiredEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterUpgradeCompleted))
            {
                ClusterUpgradeCompletedEventConverter.Serialize(writer, (ClusterUpgradeCompletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterUpgradeDomainCompleted))
            {
                ClusterUpgradeDomainCompletedEventConverter.Serialize(writer, (ClusterUpgradeDomainCompletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterUpgradeRollbackCompleted))
            {
                ClusterUpgradeRollbackCompletedEventConverter.Serialize(writer, (ClusterUpgradeRollbackCompletedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterUpgradeRollbackStarted))
            {
                ClusterUpgradeRollbackStartedEventConverter.Serialize(writer, (ClusterUpgradeRollbackStartedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterUpgradeStarted))
            {
                ClusterUpgradeStartedEventConverter.Serialize(writer, (ClusterUpgradeStartedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ChaosStopped))
            {
                ChaosStoppedEventConverter.Serialize(writer, (ChaosStoppedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ChaosStarted))
            {
                ChaosStartedEventConverter.Serialize(writer, (ChaosStartedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ChaosCodePackageRestartScheduled))
            {
                ChaosCodePackageRestartScheduledEventConverter.Serialize(writer, (ChaosCodePackageRestartScheduledEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ChaosReplicaRemovalScheduled))
            {
                ChaosReplicaRemovalScheduledEventConverter.Serialize(writer, (ChaosReplicaRemovalScheduledEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ChaosPartitionSecondaryMoveScheduled))
            {
                ChaosPartitionSecondaryMoveScheduledEventConverter.Serialize(writer, (ChaosPartitionSecondaryMoveScheduledEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ChaosPartitionPrimaryMoveScheduled))
            {
                ChaosPartitionPrimaryMoveScheduledEventConverter.Serialize(writer, (ChaosPartitionPrimaryMoveScheduledEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ChaosReplicaRestartScheduled))
            {
                ChaosReplicaRestartScheduledEventConverter.Serialize(writer, (ChaosReplicaRestartScheduledEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ChaosNodeRestartScheduled))
            {
                ChaosNodeRestartScheduledEventConverter.Serialize(writer, (ChaosNodeRestartScheduledEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ReadinessProbeFailed))
            {
                ReadinessProbeFailedEventConverter.Serialize(writer, (ReadinessProbeFailedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.LivenessProbeFailed))
            {
                LivenessProbeFailedEventConverter.Serialize(writer, (LivenessProbeFailedEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.Decision))
            {
                DecisionEventConverter.Serialize(writer, (DecisionEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.ClusterUpgradesNodesComplete))
            {
                ClusterUpgradesNodesCompleteEventConverter.Serialize(writer, (ClusterUpgradesNodesCompleteEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.BRSWarningCluster))
            {
                BRSWarningClusterEventConverter.Serialize(writer, (BRSWarningClusterEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.BRSInfoCluster))
            {
                BRSInfoClusterEventConverter.Serialize(writer, (BRSInfoClusterEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.BRSErrorCluster))
            {
                BRSErrorClusterEventConverter.Serialize(writer, (BRSErrorClusterEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.BRSWarningPartition))
            {
                BRSWarningPartitionEventConverter.Serialize(writer, (BRSWarningPartitionEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.BRSInfoPartition))
            {
                BRSInfoPartitionEventConverter.Serialize(writer, (BRSInfoPartitionEvent)obj);
            }
            else if (kind.Equals(FabricEventKind.BRSErrorPartition))
            {
                BRSErrorPartitionEventConverter.Serialize(writer, (BRSErrorPartitionEvent)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }
        }
    }
}
