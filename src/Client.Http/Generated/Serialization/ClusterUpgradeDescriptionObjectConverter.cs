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
    /// Converter for <see cref="ClusterUpgradeDescriptionObject" />.
    /// </summary>
    internal class ClusterUpgradeDescriptionObjectConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterUpgradeDescriptionObject Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterUpgradeDescriptionObject GetFromJsonProperties(JsonReader reader)
        {
            var configVersion = default(string);
            var codeVersion = default(string);
            var upgradeKind = default(UpgradeKind?);
            var rollingUpgradeMode = default(UpgradeMode?);
            var upgradeReplicaSetCheckTimeoutInSeconds = default(long?);
            var forceRestart = default(bool?);
            var sortOrder = default(UpgradeSortOrder?);
            var enableDeltaHealthEvaluation = default(bool?);
            var monitoringPolicy = default(MonitoringPolicyDescription);
            var clusterHealthPolicy = default(ClusterHealthPolicy);
            var clusterUpgradeHealthPolicy = default(ClusterUpgradeHealthPolicyObject);
            var applicationHealthPolicyMap = default(ApplicationHealthPolicyMapObject);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ConfigVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    configVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("CodeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("UpgradeKind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeKind = UpgradeKindConverter.Deserialize(reader);
                }
                else if (string.Compare("RollingUpgradeMode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    rollingUpgradeMode = UpgradeModeConverter.Deserialize(reader);
                }
                else if (string.Compare("UpgradeReplicaSetCheckTimeoutInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeReplicaSetCheckTimeoutInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("ForceRestart", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    forceRestart = reader.ReadValueAsBool();
                }
                else if (string.Compare("SortOrder", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sortOrder = UpgradeSortOrderConverter.Deserialize(reader);
                }
                else if (string.Compare("EnableDeltaHealthEvaluation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    enableDeltaHealthEvaluation = reader.ReadValueAsBool();
                }
                else if (string.Compare("MonitoringPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    monitoringPolicy = MonitoringPolicyDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("ClusterHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterHealthPolicy = ClusterHealthPolicyConverter.Deserialize(reader);
                }
                else if (string.Compare("ClusterUpgradeHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterUpgradeHealthPolicy = ClusterUpgradeHealthPolicyObjectConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationHealthPolicyMap", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthPolicyMap = ApplicationHealthPolicyMapObjectConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ClusterUpgradeDescriptionObject(
                configVersion: configVersion,
                codeVersion: codeVersion,
                upgradeKind: upgradeKind,
                rollingUpgradeMode: rollingUpgradeMode,
                upgradeReplicaSetCheckTimeoutInSeconds: upgradeReplicaSetCheckTimeoutInSeconds,
                forceRestart: forceRestart,
                sortOrder: sortOrder,
                enableDeltaHealthEvaluation: enableDeltaHealthEvaluation,
                monitoringPolicy: monitoringPolicy,
                clusterHealthPolicy: clusterHealthPolicy,
                clusterUpgradeHealthPolicy: clusterUpgradeHealthPolicy,
                applicationHealthPolicyMap: applicationHealthPolicyMap);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ClusterUpgradeDescriptionObject obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UpgradeKind, "UpgradeKind", UpgradeKindConverter.Serialize);
            writer.WriteProperty(obj.RollingUpgradeMode, "RollingUpgradeMode", UpgradeModeConverter.Serialize);
            writer.WriteProperty(obj.SortOrder, "SortOrder", UpgradeSortOrderConverter.Serialize);
            if (obj.ConfigVersion != null)
            {
                writer.WriteProperty(obj.ConfigVersion, "ConfigVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CodeVersion != null)
            {
                writer.WriteProperty(obj.CodeVersion, "CodeVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.UpgradeReplicaSetCheckTimeoutInSeconds != null)
            {
                writer.WriteProperty(obj.UpgradeReplicaSetCheckTimeoutInSeconds, "UpgradeReplicaSetCheckTimeoutInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ForceRestart != null)
            {
                writer.WriteProperty(obj.ForceRestart, "ForceRestart", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.EnableDeltaHealthEvaluation != null)
            {
                writer.WriteProperty(obj.EnableDeltaHealthEvaluation, "EnableDeltaHealthEvaluation", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.MonitoringPolicy != null)
            {
                writer.WriteProperty(obj.MonitoringPolicy, "MonitoringPolicy", MonitoringPolicyDescriptionConverter.Serialize);
            }

            if (obj.ClusterHealthPolicy != null)
            {
                writer.WriteProperty(obj.ClusterHealthPolicy, "ClusterHealthPolicy", ClusterHealthPolicyConverter.Serialize);
            }

            if (obj.ClusterUpgradeHealthPolicy != null)
            {
                writer.WriteProperty(obj.ClusterUpgradeHealthPolicy, "ClusterUpgradeHealthPolicy", ClusterUpgradeHealthPolicyObjectConverter.Serialize);
            }

            if (obj.ApplicationHealthPolicyMap != null)
            {
                writer.WriteProperty(obj.ApplicationHealthPolicyMap, "ApplicationHealthPolicyMap", ApplicationHealthPolicyMapObjectConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
