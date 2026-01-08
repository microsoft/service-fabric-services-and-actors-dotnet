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
    /// Converter for <see cref="NodeTypeHealthPolicyMapItem" />.
    /// </summary>
    internal class NodeTypeHealthPolicyMapItemConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeTypeHealthPolicyMapItem Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NodeTypeHealthPolicyMapItem GetFromJsonProperties(JsonReader reader)
        {
            var key = default(string);
            var value = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Key", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    key = reader.ReadValueAsString();
                }
                else if (string.Compare("Value", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeTypeHealthPolicyMapItem(
                key: key,
                value: value);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeTypeHealthPolicyMapItem obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Key, "Key", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Value, "Value", JsonWriterExtensions.WriteIntValue);
            writer.WriteEndObject();
        }
    }
}
