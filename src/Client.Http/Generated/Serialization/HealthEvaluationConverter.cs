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
    /// Converter for <see cref="HealthEvaluation" />.
    /// </summary>
    internal class HealthEvaluationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static HealthEvaluation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static HealthEvaluation GetFromJsonProperties(JsonReader reader)
        {
            HealthEvaluation obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Kind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Application", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Applications", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationsHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ApplicationTypeApplications", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationTypeApplicationsHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeltaNodesCheck", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeltaNodesCheckHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeployedApplication", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedApplicationHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeployedApplications", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedApplicationsHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeployedServicePackage", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedServicePackageHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DeployedServicePackages", StringComparison.OrdinalIgnoreCase))
            {
                obj = DeployedServicePackagesHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Event", StringComparison.OrdinalIgnoreCase))
            {
                obj = EventHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Node", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Nodes", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodesHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Partition", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Partitions", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionsHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Replica", StringComparison.OrdinalIgnoreCase))
            {
                obj = ReplicaHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Replicas", StringComparison.OrdinalIgnoreCase))
            {
                obj = ReplicasHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Service", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServiceHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Services", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServicesHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("SystemApplication", StringComparison.OrdinalIgnoreCase))
            {
                obj = SystemApplicationHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("UpgradeDomainDeltaNodesCheck", StringComparison.OrdinalIgnoreCase))
            {
                obj = UpgradeDomainDeltaNodesCheckHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("UpgradeDomainDeployedApplications", StringComparison.OrdinalIgnoreCase))
            {
                obj = UpgradeDomainDeployedApplicationsHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("UpgradeDomainNodes", StringComparison.OrdinalIgnoreCase))
            {
                obj = UpgradeDomainNodesHealthEvaluationConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("NodeTypeNodes", StringComparison.OrdinalIgnoreCase))
            {
                obj = NodeTypeNodesHealthEvaluationConverter.GetFromJsonProperties(reader);
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
        internal static void Serialize(JsonWriter writer, HealthEvaluation obj)
        {
            var kind = obj.Kind;
            if (kind.Equals(HealthEvaluationKind.Application))
            {
                ApplicationHealthEvaluationConverter.Serialize(writer, (ApplicationHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Applications))
            {
                ApplicationsHealthEvaluationConverter.Serialize(writer, (ApplicationsHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.ApplicationTypeApplications))
            {
                ApplicationTypeApplicationsHealthEvaluationConverter.Serialize(writer, (ApplicationTypeApplicationsHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.DeltaNodesCheck))
            {
                DeltaNodesCheckHealthEvaluationConverter.Serialize(writer, (DeltaNodesCheckHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.DeployedApplication))
            {
                DeployedApplicationHealthEvaluationConverter.Serialize(writer, (DeployedApplicationHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.DeployedApplications))
            {
                DeployedApplicationsHealthEvaluationConverter.Serialize(writer, (DeployedApplicationsHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.DeployedServicePackage))
            {
                DeployedServicePackageHealthEvaluationConverter.Serialize(writer, (DeployedServicePackageHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.DeployedServicePackages))
            {
                DeployedServicePackagesHealthEvaluationConverter.Serialize(writer, (DeployedServicePackagesHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Event))
            {
                EventHealthEvaluationConverter.Serialize(writer, (EventHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Node))
            {
                NodeHealthEvaluationConverter.Serialize(writer, (NodeHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Nodes))
            {
                NodesHealthEvaluationConverter.Serialize(writer, (NodesHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Partition))
            {
                PartitionHealthEvaluationConverter.Serialize(writer, (PartitionHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Partitions))
            {
                PartitionsHealthEvaluationConverter.Serialize(writer, (PartitionsHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Replica))
            {
                ReplicaHealthEvaluationConverter.Serialize(writer, (ReplicaHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Replicas))
            {
                ReplicasHealthEvaluationConverter.Serialize(writer, (ReplicasHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Service))
            {
                ServiceHealthEvaluationConverter.Serialize(writer, (ServiceHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.Services))
            {
                ServicesHealthEvaluationConverter.Serialize(writer, (ServicesHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.SystemApplication))
            {
                SystemApplicationHealthEvaluationConverter.Serialize(writer, (SystemApplicationHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.UpgradeDomainDeltaNodesCheck))
            {
                UpgradeDomainDeltaNodesCheckHealthEvaluationConverter.Serialize(writer, (UpgradeDomainDeltaNodesCheckHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.UpgradeDomainDeployedApplications))
            {
                UpgradeDomainDeployedApplicationsHealthEvaluationConverter.Serialize(writer, (UpgradeDomainDeployedApplicationsHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.UpgradeDomainNodes))
            {
                UpgradeDomainNodesHealthEvaluationConverter.Serialize(writer, (UpgradeDomainNodesHealthEvaluation)obj);
            }
            else if (kind.Equals(HealthEvaluationKind.NodeTypeNodes))
            {
                NodeTypeNodesHealthEvaluationConverter.Serialize(writer, (NodeTypeNodesHealthEvaluation)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown Kind.");
            }
        }
    }
}
