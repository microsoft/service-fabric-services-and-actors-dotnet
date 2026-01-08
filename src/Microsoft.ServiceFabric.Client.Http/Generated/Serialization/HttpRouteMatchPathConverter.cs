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
    /// Converter for <see cref="HttpRouteMatchPath" />.
    /// </summary>
    internal class HttpRouteMatchPathConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static HttpRouteMatchPath Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static HttpRouteMatchPath GetFromJsonProperties(JsonReader reader)
        {
            var value = default(string);
            var rewrite = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("value", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = reader.ReadValueAsString();
                }
                else if (string.Compare("rewrite", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    rewrite = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new HttpRouteMatchPath(
                value: value,
                rewrite: rewrite);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, HttpRouteMatchPath obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Value, "value", JsonWriterExtensions.WriteStringValue);
            if (obj.Rewrite != null)
            {
                writer.WriteProperty(obj.Rewrite, "rewrite", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
