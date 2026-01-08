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
    /// Converter for <see cref="NodeTransitionResult" />.
    /// </summary>
    internal class NodeTransitionResultConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeTransitionResult Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NodeTransitionResult GetFromJsonProperties(JsonReader reader)
        {
            var errorCode = default(int?);
            var nodeResult = default(NodeResult);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ErrorCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    errorCode = reader.ReadValueAsInt();
                }
                else if (string.Compare("NodeResult", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeResult = NodeResultConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeTransitionResult(
                errorCode: errorCode,
                nodeResult: nodeResult);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeTransitionResult obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ErrorCode != null)
            {
                writer.WriteProperty(obj.ErrorCode, "ErrorCode", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.NodeResult != null)
            {
                writer.WriteProperty(obj.NodeResult, "NodeResult", NodeResultConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
