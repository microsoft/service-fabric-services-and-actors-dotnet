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
    /// Converter for <see cref="AadMetadataObject" />.
    /// </summary>
    internal class AadMetadataObjectConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AadMetadataObject Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AadMetadataObject GetFromJsonProperties(JsonReader reader)
        {
            var type = default(string);
            var metadata = default(AadMetadata);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("type", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    type = reader.ReadValueAsString();
                }
                else if (string.Compare("metadata", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    metadata = AadMetadataConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new AadMetadataObject(
                type: type,
                metadata: metadata);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AadMetadataObject obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Type != null)
            {
                writer.WriteProperty(obj.Type, "type", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Metadata != null)
            {
                writer.WriteProperty(obj.Metadata, "metadata", AadMetadataConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
