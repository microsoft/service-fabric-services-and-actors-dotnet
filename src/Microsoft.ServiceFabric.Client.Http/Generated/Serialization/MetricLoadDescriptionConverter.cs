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
    /// Converter for <see cref="MetricLoadDescription" />.
    /// </summary>
    internal class MetricLoadDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static MetricLoadDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static MetricLoadDescription GetFromJsonProperties(JsonReader reader)
        {
            var metricName = default(string);
            var currentLoad = default(long?);
            var predictedLoad = default(long?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("MetricName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    metricName = reader.ReadValueAsString();
                }
                else if (string.Compare("CurrentLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentLoad = reader.ReadValueAsLong();
                }
                else if (string.Compare("PredictedLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    predictedLoad = reader.ReadValueAsLong();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new MetricLoadDescription(
                metricName: metricName,
                currentLoad: currentLoad,
                predictedLoad: predictedLoad);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, MetricLoadDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.MetricName != null)
            {
                writer.WriteProperty(obj.MetricName, "MetricName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CurrentLoad != null)
            {
                writer.WriteProperty(obj.CurrentLoad, "CurrentLoad", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.PredictedLoad != null)
            {
                writer.WriteProperty(obj.PredictedLoad, "PredictedLoad", JsonWriterExtensions.WriteLongValue);
            }

            writer.WriteEndObject();
        }
    }
}
