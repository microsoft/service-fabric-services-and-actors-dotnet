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
    /// Converter for <see cref="AnalysisEventMetadata" />.
    /// </summary>
    internal class AnalysisEventMetadataConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AnalysisEventMetadata Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AnalysisEventMetadata GetFromJsonProperties(JsonReader reader)
        {
            var delay = default(TimeSpan?);
            var duration = default(TimeSpan?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Delay", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    delay = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("Duration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    duration = reader.ReadValueAsTimeSpan();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new AnalysisEventMetadata(
                delay: delay,
                duration: duration);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AnalysisEventMetadata obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Delay != null)
            {
                writer.WriteProperty(obj.Delay, "Delay", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.Duration != null)
            {
                writer.WriteProperty(obj.Duration, "Duration", JsonWriterExtensions.WriteTimeSpanValue);
            }

            writer.WriteEndObject();
        }
    }
}
