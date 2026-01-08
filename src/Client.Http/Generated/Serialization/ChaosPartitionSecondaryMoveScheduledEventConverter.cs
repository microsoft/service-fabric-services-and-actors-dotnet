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
    /// Converter for <see cref="ChaosPartitionSecondaryMoveScheduledEvent" />.
    /// </summary>
    internal class ChaosPartitionSecondaryMoveScheduledEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosPartitionSecondaryMoveScheduledEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosPartitionSecondaryMoveScheduledEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var partitionId = default(PartitionId);
            var faultGroupId = default(Guid?);
            var faultId = default(Guid?);
            var serviceName = default(string);
            var sourceNode = default(NodeName);
            var destinationNode = default(NodeName);
            var forcedMove = default(bool?);

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
                else if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = PartitionIdConverter.Deserialize(reader);
                }
                else if (string.Compare("FaultGroupId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    faultGroupId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("FaultId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    faultId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("ServiceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = reader.ReadValueAsString();
                }
                else if (string.Compare("SourceNode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sourceNode = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("DestinationNode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    destinationNode = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("ForcedMove", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    forcedMove = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ChaosPartitionSecondaryMoveScheduledEvent(
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                partitionId: partitionId,
                faultGroupId: faultGroupId,
                faultId: faultId,
                serviceName: serviceName,
                sourceNode: sourceNode,
                destinationNode: destinationNode,
                forcedMove: forcedMove);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ChaosPartitionSecondaryMoveScheduledEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            writer.WriteProperty(obj.FaultGroupId, "FaultGroupId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.FaultId, "FaultId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.ServiceName, "ServiceName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.SourceNode, "SourceNode", NodeNameConverter.Serialize);
            writer.WriteProperty(obj.DestinationNode, "DestinationNode", NodeNameConverter.Serialize);
            writer.WriteProperty(obj.ForcedMove, "ForcedMove", JsonWriterExtensions.WriteBoolValue);
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
