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
    /// Converter for <see cref="HttpConfig" />.
    /// </summary>
    internal class HttpConfigConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static HttpConfig Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static HttpConfig GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var port = default(int?);
            var hosts = default(IEnumerable<HttpHostConfig>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("port", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    port = reader.ReadValueAsInt();
                }
                else if (string.Compare("hosts", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hosts = reader.ReadList(HttpHostConfigConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new HttpConfig(
                name: name,
                port: port,
                hosts: hosts);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, HttpConfig obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Name, "name", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Port, "port", JsonWriterExtensions.WriteIntValue);
            writer.WriteEnumerableProperty(obj.Hosts, "hosts", HttpHostConfigConverter.Serialize);
            writer.WriteEndObject();
        }
    }
}
