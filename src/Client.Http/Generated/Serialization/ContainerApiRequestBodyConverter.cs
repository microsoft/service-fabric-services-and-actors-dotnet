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
    /// Converter for <see cref="ContainerApiRequestBody" />.
    /// </summary>
    internal class ContainerApiRequestBodyConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerApiRequestBody Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerApiRequestBody GetFromJsonProperties(JsonReader reader)
        {
            var httpVerb = default(string);
            var uriPath = default(string);
            var contentType = default(string);
            var body = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("HttpVerb", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    httpVerb = reader.ReadValueAsString();
                }
                else if (string.Compare("UriPath", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    uriPath = reader.ReadValueAsString();
                }
                else if (string.Compare("Content-Type", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    contentType = reader.ReadValueAsString();
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

            return new ContainerApiRequestBody(
                httpVerb: httpVerb,
                uriPath: uriPath,
                contentType: contentType,
                body: body);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ContainerApiRequestBody obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UriPath, "UriPath", JsonWriterExtensions.WriteStringValue);
            if (obj.HttpVerb != null)
            {
                writer.WriteProperty(obj.HttpVerb, "HttpVerb", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ContentType != null)
            {
                writer.WriteProperty(obj.ContentType, "Content-Type", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Body != null)
            {
                writer.WriteProperty(obj.Body, "Body", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
