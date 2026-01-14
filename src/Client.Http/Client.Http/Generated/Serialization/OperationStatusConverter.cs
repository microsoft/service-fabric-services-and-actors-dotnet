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
    /// Converter for <see cref="OperationStatus" />.
    /// </summary>
    internal class OperationStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static OperationStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static OperationStatus GetFromJsonProperties(JsonReader reader)
        {
            var operationId = default(Guid?);
            var state = default(OperationState?);
            var type = default(OperationType?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("OperationId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    operationId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("State", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    state = OperationStateConverter.Deserialize(reader);
                }
                else if (string.Compare("Type", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    type = OperationTypeConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new OperationStatus(
                operationId: operationId,
                state: state,
                type: type);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, OperationStatus obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.State, "State", OperationStateConverter.Serialize);
            writer.WriteProperty(obj.Type, "Type", OperationTypeConverter.Serialize);
            if (obj.OperationId != null)
            {
                writer.WriteProperty(obj.OperationId, "OperationId", JsonWriterExtensions.WriteGuidValue);
            }

            writer.WriteEndObject();
        }
    }
}
