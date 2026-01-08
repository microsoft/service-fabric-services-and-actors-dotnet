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
    /// Converter for <see cref="ContainerApiResult" />.
    /// </summary>
    internal class ContainerApiResultConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerApiResult Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerApiResult GetFromJsonProperties(JsonReader reader)
        {
            var status = default(int?);
            var contentType = default(string);
            var contentEncoding = default(string);
            var body = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = reader.ReadValueAsInt();
                }
                else if (string.Compare("Content-Type", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    contentType = reader.ReadValueAsString();
                }
                else if (string.Compare("Content-Encoding", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    contentEncoding = reader.ReadValueAsString();
                }
                else if (string.Compare("Body", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    body = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ContainerApiResult(
                status: status,
                contentType: contentType,
                contentEncoding: contentEncoding,
                body: body);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ContainerApiResult obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Status, "Status", JsonWriterExtensions.WriteIntValue);
            if (obj.ContentType != null)
            {
                writer.WriteProperty(obj.ContentType, "Content-Type", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ContentEncoding != null)
            {
                writer.WriteProperty(obj.ContentEncoding, "Content-Encoding", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Body != null)
            {
                writer.WriteProperty(obj.Body, "Body", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
