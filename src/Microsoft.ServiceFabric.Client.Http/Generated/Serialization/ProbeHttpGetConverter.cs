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
    /// Converter for <see cref="ProbeHttpGet" />.
    /// </summary>
    internal class ProbeHttpGetConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ProbeHttpGet Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ProbeHttpGet GetFromJsonProperties(JsonReader reader)
        {
            var port = default(int?);
            var path = default(string);
            var host = default(string);
            var httpHeaders = default(IEnumerable<ProbeHttpGetHeaders>);
            var scheme = default(Scheme?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("port", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    port = reader.ReadValueAsInt();
                }
                else if (string.Compare("path", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    path = reader.ReadValueAsString();
                }
                else if (string.Compare("host", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    host = reader.ReadValueAsString();
                }
                else if (string.Compare("httpHeaders", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    httpHeaders = reader.ReadList(ProbeHttpGetHeadersConverter.Deserialize);
                }
                else if (string.Compare("scheme", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scheme = SchemeConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ProbeHttpGet(
                port: port,
                path: path,
                host: host,
                httpHeaders: httpHeaders,
                scheme: scheme);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ProbeHttpGet obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Port, "port", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.Scheme, "scheme", SchemeConverter.Serialize);
            if (obj.Path != null)
            {
                writer.WriteProperty(obj.Path, "path", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Host != null)
            {
                writer.WriteProperty(obj.Host, "host", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HttpHeaders != null)
            {
                writer.WriteEnumerableProperty(obj.HttpHeaders, "httpHeaders", ProbeHttpGetHeadersConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
