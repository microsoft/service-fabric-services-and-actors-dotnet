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
    /// Converter for <see cref="PropertyMetadata" />.
    /// </summary>
    internal class PropertyMetadataConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PropertyMetadata Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PropertyMetadata GetFromJsonProperties(JsonReader reader)
        {
            var typeId = default(PropertyValueKind?);
            var customTypeId = default(string);
            var parent = default(FabricName);
            var sizeInBytes = default(int?);
            var lastModifiedUtcTimestamp = default(DateTime?);
            var sequenceNumber = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("TypeId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    typeId = PropertyValueKindConverter.Deserialize(reader);
                }
                else if (string.Compare("CustomTypeId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    customTypeId = reader.ReadValueAsString();
                }
                else if (string.Compare("Parent", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    parent = FabricNameConverter.Deserialize(reader);
                }
                else if (string.Compare("SizeInBytes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sizeInBytes = reader.ReadValueAsInt();
                }
                else if (string.Compare("LastModifiedUtcTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastModifiedUtcTimestamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("SequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sequenceNumber = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PropertyMetadata(
                typeId: typeId,
                customTypeId: customTypeId,
                parent: parent,
                sizeInBytes: sizeInBytes,
                lastModifiedUtcTimestamp: lastModifiedUtcTimestamp,
                sequenceNumber: sequenceNumber);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PropertyMetadata obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.TypeId, "TypeId", PropertyValueKindConverter.Serialize);
            if (obj.CustomTypeId != null)
            {
                writer.WriteProperty(obj.CustomTypeId, "CustomTypeId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Parent != null)
            {
                writer.WriteProperty(obj.Parent, "Parent", FabricNameConverter.Serialize);
            }

            if (obj.SizeInBytes != null)
            {
                writer.WriteProperty(obj.SizeInBytes, "SizeInBytes", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.LastModifiedUtcTimestamp != null)
            {
                writer.WriteProperty(obj.LastModifiedUtcTimestamp, "LastModifiedUtcTimestamp", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.SequenceNumber != null)
            {
                writer.WriteProperty(obj.SequenceNumber, "SequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
