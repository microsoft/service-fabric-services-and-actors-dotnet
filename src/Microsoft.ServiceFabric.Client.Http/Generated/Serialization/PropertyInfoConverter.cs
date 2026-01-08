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
    /// Converter for <see cref="PropertyInfo" />.
    /// </summary>
    internal class PropertyInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PropertyInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PropertyInfo GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var value = default(PropertyValue);
            var metadata = default(PropertyMetadata);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("Value", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = PropertyValueConverter.Deserialize(reader);
                }
                else if (string.Compare("Metadata", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    metadata = PropertyMetadataConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PropertyInfo(
                name: name,
                value: value,
                metadata: metadata);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PropertyInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Metadata, "Metadata", PropertyMetadataConverter.Serialize);
            if (obj.Value != null)
            {
                writer.WriteProperty(obj.Value, "Value", PropertyValueConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
