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
    /// Converter for <see cref="PartitionEvent" />.
    /// </summary>
    internal class PartitionEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var partitionId = default(PartitionId);

            do
            {
                var propName = reader.ReadPropertyName();
                if (propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
                {
                    var propValue = reader.ReadValueAsString();

                    if (propValue.Equals("PartitionAnalysisEvent", StringComparison.OrdinalIgnoreCase))
                    {
                        return PartitionAnalysisEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("PartitionNewHealthReport", StringComparison.OrdinalIgnoreCase))
                    {
                        return PartitionNewHealthReportEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("PartitionHealthReportExpired", StringComparison.OrdinalIgnoreCase))
                    {
                        return PartitionHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("PartitionReconfigured", StringComparison.OrdinalIgnoreCase))
                    {
                        return PartitionReconfiguredEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("PartitionPrimaryMoveAnalysis", StringComparison.OrdinalIgnoreCase))
                    {
                        return PartitionPrimaryMoveAnalysisEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ChaosPartitionSecondaryMoveScheduled", StringComparison.OrdinalIgnoreCase))
                    {
                        return ChaosPartitionSecondaryMoveScheduledEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ChaosPartitionPrimaryMoveScheduled", StringComparison.OrdinalIgnoreCase))
                    {
                        return ChaosPartitionPrimaryMoveScheduledEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("BRSWarningPartition", StringComparison.OrdinalIgnoreCase))
                    {
                        return BRSWarningPartitionEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("BRSInfoPartition", StringComparison.OrdinalIgnoreCase))
                    {
                        return BRSInfoPartitionEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("BRSErrorPartition", StringComparison.OrdinalIgnoreCase))
                    {
                        return BRSErrorPartitionEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("PartitionEvent", StringComparison.OrdinalIgnoreCase))
                    {
                        // kind specified as same type, deserialize using properties.
                    }
                    else
                    {
                        throw new InvalidOperationException("Unknown Discriminator.");
                    }
                }
                else
                {
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
                    else
                    {
                        reader.SkipPropertyValue();
                    }
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PartitionEvent(
                kind: Common.FabricEventKind.PartitionEvent,
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                partitionId: partitionId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
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
