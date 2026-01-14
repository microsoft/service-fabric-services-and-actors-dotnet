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
    /// Converter for <see cref="HealthEvent" />.
    /// </summary>
    internal class HealthEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static HealthEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static HealthEvent GetFromJsonProperties(JsonReader reader)
        {
            var sourceId = default(string);
            var property = default(string);
            var healthState = default(HealthState?);
            var timeToLiveInMilliSeconds = default(TimeSpan?);
            var description = default(string);
            var sequenceNumber = default(string);
            var removeWhenExpired = default(bool?);
            var healthReportId = default(string);
            var isExpired = default(bool?);
            var sourceUtcTimestamp = default(DateTime?);
            var lastModifiedUtcTimestamp = default(DateTime?);
            var lastOkTransitionAt = default(DateTime?);
            var lastWarningTransitionAt = default(DateTime?);
            var lastErrorTransitionAt = default(DateTime?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("SourceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sourceId = reader.ReadValueAsString();
                }
                else if (string.Compare("Property", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    property = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("TimeToLiveInMilliSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeToLiveInMilliSeconds = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("Description", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    description = reader.ReadValueAsString();
                }
                else if (string.Compare("SequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("RemoveWhenExpired", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    removeWhenExpired = reader.ReadValueAsBool();
                }
                else if (string.Compare("HealthReportId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthReportId = reader.ReadValueAsString();
                }
                else if (string.Compare("IsExpired", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isExpired = reader.ReadValueAsBool();
                }
                else if (string.Compare("SourceUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sourceUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastModifiedUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastModifiedUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastOkTransitionAt", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastOkTransitionAt = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastWarningTransitionAt", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastWarningTransitionAt = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastErrorTransitionAt", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastErrorTransitionAt = reader.ReadValueAsDateTime();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new HealthEvent(
                sourceId: sourceId,
                property: property,
                healthState: healthState,
                timeToLiveInMilliSeconds: timeToLiveInMilliSeconds,
                description: description,
                sequenceNumber: sequenceNumber,
                removeWhenExpired: removeWhenExpired,
                healthReportId: healthReportId,
                isExpired: isExpired,
                sourceUtcTimestamp: sourceUtcTimestamp,
                lastModifiedUtcTimestamp: lastModifiedUtcTimestamp,
                lastOkTransitionAt: lastOkTransitionAt,
                lastWarningTransitionAt: lastWarningTransitionAt,
                lastErrorTransitionAt: lastErrorTransitionAt);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, HealthEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            writer.WriteProperty(obj.SourceId, "SourceId", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Property, "Property", JsonWriterExtensions.WriteStringValue);
            if (obj.TimeToLiveInMilliSeconds != null)
            {
                writer.WriteProperty(obj.TimeToLiveInMilliSeconds, "TimeToLiveInMilliSeconds", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.Description != null)
            {
                writer.WriteProperty(obj.Description, "Description", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.SequenceNumber != null)
            {
                writer.WriteProperty(obj.SequenceNumber, "SequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.RemoveWhenExpired != null)
            {
                writer.WriteProperty(obj.RemoveWhenExpired, "RemoveWhenExpired", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.HealthReportId != null)
            {
                writer.WriteProperty(obj.HealthReportId, "HealthReportId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.IsExpired != null)
            {
                writer.WriteProperty(obj.IsExpired, "IsExpired", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.SourceUtcTimestamp != null)
            {
                writer.WriteProperty(obj.SourceUtcTimestamp, "SourceUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastModifiedUtcTimestamp != null)
            {
                writer.WriteProperty(obj.LastModifiedUtcTimestamp, "LastModifiedUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastOkTransitionAt != null)
            {
                writer.WriteProperty(obj.LastOkTransitionAt, "LastOkTransitionAt", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastWarningTransitionAt != null)
            {
                writer.WriteProperty(obj.LastWarningTransitionAt, "LastWarningTransitionAt", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastErrorTransitionAt != null)
            {
                writer.WriteProperty(obj.LastErrorTransitionAt, "LastErrorTransitionAt", JsonWriterExtensions.WriteDateTimeValue);
            }

            writer.WriteEndObject();
        }
    }
}
