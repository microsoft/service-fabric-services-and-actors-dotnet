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
    /// Converter for <see cref="UniformInt64RangePartitionSchemeDescription" />.
    /// </summary>
    internal class UniformInt64RangePartitionSchemeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static UniformInt64RangePartitionSchemeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static UniformInt64RangePartitionSchemeDescription GetFromJsonProperties(JsonReader reader)
        {
            var count = default(int?);
            var lowKey = default(string);
            var highKey = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Count", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    count = reader.ReadValueAsInt();
                }
                else if (string.Compare("LowKey", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lowKey = reader.ReadValueAsString();
                }
                else if (string.Compare("HighKey", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    highKey = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new UniformInt64RangePartitionSchemeDescription(
                count: count,
                lowKey: lowKey,
                highKey: highKey);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, UniformInt64RangePartitionSchemeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.PartitionScheme, "PartitionScheme", PartitionSchemeConverter.Serialize);
            writer.WriteProperty(obj.Count, "Count", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.LowKey, "LowKey", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.HighKey, "HighKey", JsonWriterExtensions.WriteStringValue);
            writer.WriteEndObject();
        }
    }
}
