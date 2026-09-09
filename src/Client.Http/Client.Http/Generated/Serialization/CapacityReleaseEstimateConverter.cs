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
    /// Converter for <see cref="CapacityReleaseEstimate" />.
    /// </summary>
    internal class CapacityReleaseEstimateConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static CapacityReleaseEstimate Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static CapacityReleaseEstimate GetFromJsonProperties(JsonReader reader)
        {
            var level = default(CapacityReleaseLevel?);
            var metricName = default(string);
            var usedCapacity = default(long?);
            var totalCapacity = default(long?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Level", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    level = CapacityReleaseLevelConverter.Deserialize(reader);
                }
                else if (string.Compare("MetricName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    metricName = reader.ReadValueAsString();
                }
                else if (string.Compare("UsedCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    usedCapacity = reader.ReadValueAsLong();
                }
                else if (string.Compare("TotalCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    totalCapacity = reader.ReadValueAsLong();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new CapacityReleaseEstimate(
                level: level,
                metricName: metricName,
                usedCapacity: usedCapacity,
                totalCapacity: totalCapacity);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, CapacityReleaseEstimate obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Level, "Level", CapacityReleaseLevelConverter.Serialize);
            writer.WriteProperty(obj.MetricName, "MetricName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.UsedCapacity, "UsedCapacity", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.TotalCapacity, "TotalCapacity", JsonWriterExtensions.WriteLongValue);
            writer.WriteEndObject();
        }
    }
}
