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
    /// Converter for <see cref="HealthInformation" />.
    /// </summary>
    internal class HealthInformationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static HealthInformation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static HealthInformation GetFromJsonProperties(JsonReader reader)
        {
            var sourceId = default(string);
            var property = default(string);
            var healthState = default(HealthState?);
            var timeToLiveInMilliSeconds = default(TimeSpan?);
            var description = default(string);
            var sequenceNumber = default(string);
            var removeWhenExpired = default(bool?);
            var healthReportId = default(string);

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
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new HealthInformation(
                sourceId: sourceId,
                property: property,
                healthState: healthState,
                timeToLiveInMilliSeconds: timeToLiveInMilliSeconds,
                description: description,
                sequenceNumber: sequenceNumber,
                removeWhenExpired: removeWhenExpired,
                healthReportId: healthReportId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, HealthInformation obj)
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

            writer.WriteEndObject();
        }
    }
}
