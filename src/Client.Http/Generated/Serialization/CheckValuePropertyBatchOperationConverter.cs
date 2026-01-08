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
    /// Converter for <see cref="CheckValuePropertyBatchOperation" />.
    /// </summary>
    internal class CheckValuePropertyBatchOperationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static CheckValuePropertyBatchOperation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static CheckValuePropertyBatchOperation GetFromJsonProperties(JsonReader reader)
        {
            var propertyName = default(string);
            var value = default(PropertyValue);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PropertyName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    propertyName = reader.ReadValueAsString();
                }
                else if (string.Compare("Value", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = PropertyValueConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new CheckValuePropertyBatchOperation(
                propertyName: propertyName,
                value: value);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, CheckValuePropertyBatchOperation obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", PropertyBatchOperationKindConverter.Serialize);
            writer.WriteProperty(obj.PropertyName, "PropertyName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Value, "Value", PropertyValueConverter.Serialize);
            writer.WriteEndObject();
        }
    }
}
