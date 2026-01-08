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
    /// Converter for <see cref="DiagnosticsDescription" />.
    /// </summary>
    internal class DiagnosticsDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DiagnosticsDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DiagnosticsDescription GetFromJsonProperties(JsonReader reader)
        {
            var sinks = default(IEnumerable<DiagnosticsSinkProperties>);
            var enabled = default(bool?);
            var defaultSinkRefs = default(IEnumerable<string>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("sinks", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sinks = reader.ReadList(DiagnosticsSinkPropertiesConverter.Deserialize);
                }
                else if (string.Compare("enabled", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    enabled = reader.ReadValueAsBool();
                }
                else if (string.Compare("defaultSinkRefs", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    defaultSinkRefs = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DiagnosticsDescription(
                sinks: sinks,
                enabled: enabled,
                defaultSinkRefs: defaultSinkRefs);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DiagnosticsDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Sinks != null)
            {
                writer.WriteEnumerableProperty(obj.Sinks, "sinks", DiagnosticsSinkPropertiesConverter.Serialize);
            }

            if (obj.Enabled != null)
            {
                writer.WriteProperty(obj.Enabled, "enabled", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.DefaultSinkRefs != null)
            {
                writer.WriteEnumerableProperty(obj.DefaultSinkRefs, "defaultSinkRefs", (w, v) => writer.WriteStringValue(v));
            }

            writer.WriteEndObject();
        }
    }
}
