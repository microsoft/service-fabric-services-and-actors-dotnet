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
    /// Converter for <see cref="DiagnosticsRef" />.
    /// </summary>
    internal class DiagnosticsRefConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DiagnosticsRef Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DiagnosticsRef GetFromJsonProperties(JsonReader reader)
        {
            var enabled = default(bool?);
            var sinkRefs = default(IEnumerable<string>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("enabled", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    enabled = reader.ReadValueAsBool();
                }
                else if (string.Compare("sinkRefs", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sinkRefs = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DiagnosticsRef(
                enabled: enabled,
                sinkRefs: sinkRefs);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DiagnosticsRef obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Enabled != null)
            {
                writer.WriteProperty(obj.Enabled, "enabled", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.SinkRefs != null)
            {
                writer.WriteEnumerableProperty(obj.SinkRefs, "sinkRefs", (w, v) => writer.WriteStringValue(v));
            }

            writer.WriteEndObject();
        }
    }
}
