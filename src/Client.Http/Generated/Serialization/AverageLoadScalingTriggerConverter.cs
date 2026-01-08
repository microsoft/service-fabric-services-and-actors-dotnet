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
    /// Converter for <see cref="AverageLoadScalingTrigger" />.
    /// </summary>
    internal class AverageLoadScalingTriggerConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AverageLoadScalingTrigger Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AverageLoadScalingTrigger GetFromJsonProperties(JsonReader reader)
        {
            var metric = default(AutoScalingMetric);
            var lowerLoadThreshold = default(double?);
            var upperLoadThreshold = default(double?);
            var scaleIntervalInSeconds = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("metric", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    metric = AutoScalingMetricConverter.Deserialize(reader);
                }
                else if (string.Compare("lowerLoadThreshold", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lowerLoadThreshold = reader.ReadValueAsDouble();
                }
                else if (string.Compare("upperLoadThreshold", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upperLoadThreshold = reader.ReadValueAsDouble();
                }
                else if (string.Compare("scaleIntervalInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scaleIntervalInSeconds = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new AverageLoadScalingTrigger(
                metric: metric,
                lowerLoadThreshold: lowerLoadThreshold,
                upperLoadThreshold: upperLoadThreshold,
                scaleIntervalInSeconds: scaleIntervalInSeconds);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AverageLoadScalingTrigger obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "kind", AutoScalingTriggerKindConverter.Serialize);
            writer.WriteProperty(obj.Metric, "metric", AutoScalingMetricConverter.Serialize);
            writer.WriteProperty(obj.LowerLoadThreshold, "lowerLoadThreshold", JsonWriterExtensions.WriteDoubleValue);
            writer.WriteProperty(obj.UpperLoadThreshold, "upperLoadThreshold", JsonWriterExtensions.WriteDoubleValue);
            writer.WriteProperty(obj.ScaleIntervalInSeconds, "scaleIntervalInSeconds", JsonWriterExtensions.WriteIntValue);
            writer.WriteEndObject();
        }
    }
}
