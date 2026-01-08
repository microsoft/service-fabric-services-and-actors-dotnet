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
    /// Converter for <see cref="UpdatePartitionLoadResult" />.
    /// </summary>
    internal class UpdatePartitionLoadResultConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static UpdatePartitionLoadResult Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static UpdatePartitionLoadResult GetFromJsonProperties(JsonReader reader)
        {
            var partitionId = default(PartitionId);
            var partitionErrorCode = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = PartitionIdConverter.Deserialize(reader);
                }
                else if (string.Compare("PartitionErrorCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionErrorCode = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new UpdatePartitionLoadResult(
                partitionId: partitionId,
                partitionErrorCode: partitionErrorCode);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, UpdatePartitionLoadResult obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.PartitionId != null)
            {
                writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            }

            if (obj.PartitionErrorCode != null)
            {
                writer.WriteProperty(obj.PartitionErrorCode, "PartitionErrorCode", JsonWriterExtensions.WriteIntValue);
            }

            writer.WriteEndObject();
        }
    }
}
