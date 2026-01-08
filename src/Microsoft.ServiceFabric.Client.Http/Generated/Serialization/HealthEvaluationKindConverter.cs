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
    /// Converter for <see cref="HealthEvaluationKind" />.
    /// </summary>
    internal class HealthEvaluationKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static HealthEvaluationKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(HealthEvaluationKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Invalid;
            }
            else if (string.Compare(value, "Event", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Event;
            }
            else if (string.Compare(value, "Replicas", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Replicas;
            }
            else if (string.Compare(value, "Partitions", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Partitions;
            }
            else if (string.Compare(value, "DeployedServicePackages", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.DeployedServicePackages;
            }
            else if (string.Compare(value, "DeployedApplications", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.DeployedApplications;
            }
            else if (string.Compare(value, "Services", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Services;
            }
            else if (string.Compare(value, "Nodes", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Nodes;
            }
            else if (string.Compare(value, "Applications", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Applications;
            }
            else if (string.Compare(value, "SystemApplication", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.SystemApplication;
            }
            else if (string.Compare(value, "UpgradeDomainDeployedApplications", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.UpgradeDomainDeployedApplications;
            }
            else if (string.Compare(value, "UpgradeDomainNodes", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.UpgradeDomainNodes;
            }
            else if (string.Compare(value, "Replica", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Replica;
            }
            else if (string.Compare(value, "Partition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Partition;
            }
            else if (string.Compare(value, "DeployedServicePackage", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.DeployedServicePackage;
            }
            else if (string.Compare(value, "DeployedApplication", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.DeployedApplication;
            }
            else if (string.Compare(value, "Service", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Service;
            }
            else if (string.Compare(value, "Node", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Node;
            }
            else if (string.Compare(value, "Application", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.Application;
            }
            else if (string.Compare(value, "DeltaNodesCheck", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.DeltaNodesCheck;
            }
            else if (string.Compare(value, "UpgradeDomainDeltaNodesCheck", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.UpgradeDomainDeltaNodesCheck;
            }
            else if (string.Compare(value, "ApplicationTypeApplications", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.ApplicationTypeApplications;
            }
            else if (string.Compare(value, "NodeTypeNodes", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthEvaluationKind.NodeTypeNodes;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, HealthEvaluationKind? value)
        {
            switch (value)
            {
                case HealthEvaluationKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case HealthEvaluationKind.Event:
                    writer.WriteStringValue("Event");
                    break;
                case HealthEvaluationKind.Replicas:
                    writer.WriteStringValue("Replicas");
                    break;
                case HealthEvaluationKind.Partitions:
                    writer.WriteStringValue("Partitions");
                    break;
                case HealthEvaluationKind.DeployedServicePackages:
                    writer.WriteStringValue("DeployedServicePackages");
                    break;
                case HealthEvaluationKind.DeployedApplications:
                    writer.WriteStringValue("DeployedApplications");
                    break;
                case HealthEvaluationKind.Services:
                    writer.WriteStringValue("Services");
                    break;
                case HealthEvaluationKind.Nodes:
                    writer.WriteStringValue("Nodes");
                    break;
                case HealthEvaluationKind.Applications:
                    writer.WriteStringValue("Applications");
                    break;
                case HealthEvaluationKind.SystemApplication:
                    writer.WriteStringValue("SystemApplication");
                    break;
                case HealthEvaluationKind.UpgradeDomainDeployedApplications:
                    writer.WriteStringValue("UpgradeDomainDeployedApplications");
                    break;
                case HealthEvaluationKind.UpgradeDomainNodes:
                    writer.WriteStringValue("UpgradeDomainNodes");
                    break;
                case HealthEvaluationKind.Replica:
                    writer.WriteStringValue("Replica");
                    break;
                case HealthEvaluationKind.Partition:
                    writer.WriteStringValue("Partition");
                    break;
                case HealthEvaluationKind.DeployedServicePackage:
                    writer.WriteStringValue("DeployedServicePackage");
                    break;
                case HealthEvaluationKind.DeployedApplication:
                    writer.WriteStringValue("DeployedApplication");
                    break;
                case HealthEvaluationKind.Service:
                    writer.WriteStringValue("Service");
                    break;
                case HealthEvaluationKind.Node:
                    writer.WriteStringValue("Node");
                    break;
                case HealthEvaluationKind.Application:
                    writer.WriteStringValue("Application");
                    break;
                case HealthEvaluationKind.DeltaNodesCheck:
                    writer.WriteStringValue("DeltaNodesCheck");
                    break;
                case HealthEvaluationKind.UpgradeDomainDeltaNodesCheck:
                    writer.WriteStringValue("UpgradeDomainDeltaNodesCheck");
                    break;
                case HealthEvaluationKind.ApplicationTypeApplications:
                    writer.WriteStringValue("ApplicationTypeApplications");
                    break;
                case HealthEvaluationKind.NodeTypeNodes:
                    writer.WriteStringValue("NodeTypeNodes");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type HealthEvaluationKind");
            }
        }
    }
}
