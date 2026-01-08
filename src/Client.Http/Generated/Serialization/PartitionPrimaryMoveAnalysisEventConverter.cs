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
    /// Converter for <see cref="PartitionPrimaryMoveAnalysisEvent" />.
    /// </summary>
    internal class PartitionPrimaryMoveAnalysisEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionPrimaryMoveAnalysisEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionPrimaryMoveAnalysisEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var partitionId = default(PartitionId);
            var metadata = default(AnalysisEventMetadata);
            var whenMoveCompleted = default(DateTime?);
            var previousNode = default(NodeName);
            var currentNode = default(NodeName);
            var moveReason = default(string);
            var relevantTraces = default(string);

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
                else if (string.Compare("Metadata", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    metadata = AnalysisEventMetadataConverter.Deserialize(reader);
                }
                else if (string.Compare("WhenMoveCompleted", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    whenMoveCompleted = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("PreviousNode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    previousNode = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("CurrentNode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentNode = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("MoveReason", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    moveReason = reader.ReadValueAsString();
                }
                else if (string.Compare("RelevantTraces", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    relevantTraces = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PartitionPrimaryMoveAnalysisEvent(
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                partitionId: partitionId,
                metadata: metadata,
                whenMoveCompleted: whenMoveCompleted,
                previousNode: previousNode,
                currentNode: currentNode,
                moveReason: moveReason,
                relevantTraces: relevantTraces);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionPrimaryMoveAnalysisEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            writer.WriteProperty(obj.Metadata, "Metadata", AnalysisEventMetadataConverter.Serialize);
            writer.WriteProperty(obj.WhenMoveCompleted, "WhenMoveCompleted", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.PreviousNode, "PreviousNode", NodeNameConverter.Serialize);
            writer.WriteProperty(obj.CurrentNode, "CurrentNode", NodeNameConverter.Serialize);
            writer.WriteProperty(obj.MoveReason, "MoveReason", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.RelevantTraces, "RelevantTraces", JsonWriterExtensions.WriteStringValue);
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
