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
    /// Converter for <see cref="Int64RangePartitionInformation" />.
    /// </summary>
    internal class Int64RangePartitionInformationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static Int64RangePartitionInformation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static Int64RangePartitionInformation GetFromJsonProperties(JsonReader reader)
        {
            var id = default(PartitionId);
            var lowKey = default(string);
            var highKey = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Id", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    id = PartitionIdConverter.Deserialize(reader);
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

            return new Int64RangePartitionInformation(
                id: id,
                lowKey: lowKey,
                highKey: highKey);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, Int64RangePartitionInformation obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServicePartitionKind, "ServicePartitionKind", ServicePartitionKindConverter.Serialize);
            if (obj.Id != null)
            {
                writer.WriteProperty(obj.Id, "Id", PartitionIdConverter.Serialize);
            }

            if (obj.LowKey != null)
            {
                writer.WriteProperty(obj.LowKey, "LowKey", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HighKey != null)
            {
                writer.WriteProperty(obj.HighKey, "HighKey", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
