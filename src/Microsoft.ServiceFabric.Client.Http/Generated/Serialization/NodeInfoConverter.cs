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
    /// Converter for <see cref="NodeInfo" />.
    /// </summary>
    internal class NodeInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NodeInfo GetFromJsonProperties(JsonReader reader)
        {
            var name = default(NodeName);
            var ipAddressOrFQDN = default(string);
            var type = default(string);
            var codeVersion = default(string);
            var configVersion = default(string);
            var nodeStatus = default(NodeStatus?);
            var nodeUpTimeInSeconds = default(string);
            var healthState = default(HealthState?);
            var isSeedNode = default(bool?);
            var upgradeDomain = default(string);
            var faultDomain = default(string);
            var id = default(NodeId);
            var instanceId = default(string);
            var nodeDeactivationInfo = default(NodeDeactivationInfo);
            var isStopped = default(bool?);
            var nodeDownTimeInSeconds = default(string);
            var nodeUpAt = default(DateTime?);
            var nodeDownAt = default(DateTime?);
            var nodeTags = default(IEnumerable<string>);
            var isNodeByNodeUpgradeInProgress = default(bool?);
            var infrastructurePlacementID = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("IpAddressOrFQDN", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ipAddressOrFQDN = reader.ReadValueAsString();
                }
                else if (string.Compare("Type", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    type = reader.ReadValueAsString();
                }
                else if (string.Compare("CodeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("ConfigVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    configVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeStatus = NodeStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("NodeUpTimeInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeUpTimeInSeconds = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("IsSeedNode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isSeedNode = reader.ReadValueAsBool();
                }
                else if (string.Compare("UpgradeDomain", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeDomain = reader.ReadValueAsString();
                }
                else if (string.Compare("FaultDomain", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    faultDomain = reader.ReadValueAsString();
                }
                else if (string.Compare("Id", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    id = NodeIdConverter.Deserialize(reader);
                }
                else if (string.Compare("InstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceId = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeDeactivationInfo", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeDeactivationInfo = NodeDeactivationInfoConverter.Deserialize(reader);
                }
                else if (string.Compare("IsStopped", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isStopped = reader.ReadValueAsBool();
                }
                else if (string.Compare("NodeDownTimeInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeDownTimeInSeconds = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeUpAt", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeUpAt = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("NodeDownAt", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeDownAt = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("NodeTags", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeTags = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else if (string.Compare("IsNodeByNodeUpgradeInProgress", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isNodeByNodeUpgradeInProgress = reader.ReadValueAsBool();
                }
                else if (string.Compare("InfrastructurePlacementID", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    infrastructurePlacementID = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeInfo(
                name: name,
                ipAddressOrFQDN: ipAddressOrFQDN,
                type: type,
                codeVersion: codeVersion,
                configVersion: configVersion,
                nodeStatus: nodeStatus,
                nodeUpTimeInSeconds: nodeUpTimeInSeconds,
                healthState: healthState,
                isSeedNode: isSeedNode,
                upgradeDomain: upgradeDomain,
                faultDomain: faultDomain,
                id: id,
                instanceId: instanceId,
                nodeDeactivationInfo: nodeDeactivationInfo,
                isStopped: isStopped,
                nodeDownTimeInSeconds: nodeDownTimeInSeconds,
                nodeUpAt: nodeUpAt,
                nodeDownAt: nodeDownAt,
                nodeTags: nodeTags,
                isNodeByNodeUpgradeInProgress: isNodeByNodeUpgradeInProgress,
                infrastructurePlacementID: infrastructurePlacementID);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.NodeStatus, "NodeStatus", NodeStatusConverter.Serialize);
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", NodeNameConverter.Serialize);
            }

            if (obj.IpAddressOrFQDN != null)
            {
                writer.WriteProperty(obj.IpAddressOrFQDN, "IpAddressOrFQDN", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Type != null)
            {
                writer.WriteProperty(obj.Type, "Type", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CodeVersion != null)
            {
                writer.WriteProperty(obj.CodeVersion, "CodeVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ConfigVersion != null)
            {
                writer.WriteProperty(obj.ConfigVersion, "ConfigVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NodeUpTimeInSeconds != null)
            {
                writer.WriteProperty(obj.NodeUpTimeInSeconds, "NodeUpTimeInSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.IsSeedNode != null)
            {
                writer.WriteProperty(obj.IsSeedNode, "IsSeedNode", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.UpgradeDomain != null)
            {
                writer.WriteProperty(obj.UpgradeDomain, "UpgradeDomain", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FaultDomain != null)
            {
                writer.WriteProperty(obj.FaultDomain, "FaultDomain", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Id != null)
            {
                writer.WriteProperty(obj.Id, "Id", NodeIdConverter.Serialize);
            }

            if (obj.InstanceId != null)
            {
                writer.WriteProperty(obj.InstanceId, "InstanceId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NodeDeactivationInfo != null)
            {
                writer.WriteProperty(obj.NodeDeactivationInfo, "NodeDeactivationInfo", NodeDeactivationInfoConverter.Serialize);
            }

            if (obj.IsStopped != null)
            {
                writer.WriteProperty(obj.IsStopped, "IsStopped", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.NodeDownTimeInSeconds != null)
            {
                writer.WriteProperty(obj.NodeDownTimeInSeconds, "NodeDownTimeInSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NodeUpAt != null)
            {
                writer.WriteProperty(obj.NodeUpAt, "NodeUpAt", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.NodeDownAt != null)
            {
                writer.WriteProperty(obj.NodeDownAt, "NodeDownAt", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.NodeTags != null)
            {
                writer.WriteEnumerableProperty(obj.NodeTags, "NodeTags", (w, v) => writer.WriteStringValue(v));
            }

            if (obj.IsNodeByNodeUpgradeInProgress != null)
            {
                writer.WriteProperty(obj.IsNodeByNodeUpgradeInProgress, "IsNodeByNodeUpgradeInProgress", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.InfrastructurePlacementID != null)
            {
                writer.WriteProperty(obj.InfrastructurePlacementID, "InfrastructurePlacementID", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
