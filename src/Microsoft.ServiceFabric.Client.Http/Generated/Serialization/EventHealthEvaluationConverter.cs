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
    /// Converter for <see cref="EventHealthEvaluation" />.
    /// </summary>
    internal class EventHealthEvaluationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static EventHealthEvaluation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static EventHealthEvaluation GetFromJsonProperties(JsonReader reader)
        {
            var aggregatedHealthState = default(HealthState?);
            var description = default(string);
            var considerWarningAsError = default(bool?);
            var unhealthyEvent = default(HealthEvent);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("AggregatedHealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    aggregatedHealthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("Description", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    description = reader.ReadValueAsString();
                }
                else if (string.Compare("ConsiderWarningAsError", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    considerWarningAsError = reader.ReadValueAsBool();
                }
                else if (string.Compare("UnhealthyEvent", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    unhealthyEvent = HealthEventConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new EventHealthEvaluation(
                aggregatedHealthState: aggregatedHealthState,
                description: description,
                considerWarningAsError: considerWarningAsError,
                unhealthyEvent: unhealthyEvent);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, EventHealthEvaluation obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", HealthEvaluationKindConverter.Serialize);
            writer.WriteProperty(obj.AggregatedHealthState, "AggregatedHealthState", HealthStateConverter.Serialize);
            if (obj.Description != null)
            {
                writer.WriteProperty(obj.Description, "Description", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ConsiderWarningAsError != null)
            {
                writer.WriteProperty(obj.ConsiderWarningAsError, "ConsiderWarningAsError", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.UnhealthyEvent != null)
            {
                writer.WriteProperty(obj.UnhealthyEvent, "UnhealthyEvent", HealthEventConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
