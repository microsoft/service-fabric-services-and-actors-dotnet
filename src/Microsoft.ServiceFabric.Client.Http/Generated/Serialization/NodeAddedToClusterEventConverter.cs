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
    /// Converter for <see cref="NodeAddedToClusterEvent" />.
    /// </summary>
    internal class NodeAddedToClusterEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeAddedToClusterEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NodeAddedToClusterEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var nodeName = default(NodeName);
            var nodeId = default(string);
            var nodeInstance = default(long?);
            var nodeType = default(string);
            var fabricVersion = default(string);
            var ipAddressOrFQDN = default(string);
            var nodeCapacities = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("EventInstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    eventInstanceId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("Category", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    category = reader.ReadValueAsString();
                }
                else if (string.Compare("TimeStamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeStamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("HasCorrelatedEvents", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hasCorrelatedEvents = reader.ReadValueAsBool();
                }
                else if (string.Compare("NodeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeName = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("NodeId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeId = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeInstance", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeInstance = reader.ReadValueAsLong();
                }
                else if (string.Compare("NodeType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeType = reader.ReadValueAsString();
                }
                else if (string.Compare("FabricVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    fabricVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("IpAddressOrFQDN", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ipAddressOrFQDN = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeCapacities", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeCapacities = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeAddedToClusterEvent(
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                nodeName: nodeName,
                nodeId: nodeId,
                nodeInstance: nodeInstance,
                nodeType: nodeType,
                fabricVersion: fabricVersion,
                ipAddressOrFQDN: ipAddressOrFQDN,
                nodeCapacities: nodeCapacities);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeAddedToClusterEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.NodeName, "NodeName", NodeNameConverter.Serialize);
            writer.WriteProperty(obj.NodeId, "NodeId", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.NodeInstance, "NodeInstance", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.NodeType, "NodeType", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.FabricVersion, "FabricVersion", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.IpAddressOrFQDN, "IpAddressOrFQDN", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.NodeCapacities, "NodeCapacities", JsonWriterExtensions.WriteStringValue);
            if (obj.Category != null)
            {
                writer.WriteProperty(obj.Category, "Category", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HasCorrelatedEvents != null)
            {
                writer.WriteProperty(obj.HasCorrelatedEvents, "HasCorrelatedEvents", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
