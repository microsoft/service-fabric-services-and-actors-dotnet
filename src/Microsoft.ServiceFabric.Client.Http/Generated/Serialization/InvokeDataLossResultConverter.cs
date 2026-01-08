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
    /// Converter for <see cref="InvokeDataLossResult" />.
    /// </summary>
    internal class InvokeDataLossResultConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static InvokeDataLossResult Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static InvokeDataLossResult GetFromJsonProperties(JsonReader reader)
        {
            var errorCode = default(int?);
            var selectedPartition = default(SelectedPartition);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ErrorCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    errorCode = reader.ReadValueAsInt();
                }
                else if (string.Compare("SelectedPartition", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    selectedPartition = SelectedPartitionConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new InvokeDataLossResult(
                errorCode: errorCode,
                selectedPartition: selectedPartition);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, InvokeDataLossResult obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ErrorCode != null)
            {
                writer.WriteProperty(obj.ErrorCode, "ErrorCode", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.SelectedPartition != null)
            {
                writer.WriteProperty(obj.SelectedPartition, "SelectedPartition", SelectedPartitionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
