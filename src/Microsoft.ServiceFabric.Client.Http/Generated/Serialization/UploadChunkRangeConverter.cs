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
    /// Converter for <see cref="UploadChunkRange" />.
    /// </summary>
    internal class UploadChunkRangeConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static UploadChunkRange Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static UploadChunkRange GetFromJsonProperties(JsonReader reader)
        {
            var startPosition = default(string);
            var endPosition = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("StartPosition", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    startPosition = reader.ReadValueAsString();
                }
                else if (string.Compare("EndPosition", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    endPosition = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new UploadChunkRange(
                startPosition: startPosition,
                endPosition: endPosition);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, UploadChunkRange obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.StartPosition != null)
            {
                writer.WriteProperty(obj.StartPosition, "StartPosition", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.EndPosition != null)
            {
                writer.WriteProperty(obj.EndPosition, "EndPosition", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
