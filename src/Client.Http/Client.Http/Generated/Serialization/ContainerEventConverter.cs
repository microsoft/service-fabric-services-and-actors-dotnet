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
    /// Converter for <see cref="ContainerEvent" />.
    /// </summary>
    internal class ContainerEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerEvent GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var count = default(int?);
            var firstTimestamp = default(string);
            var lastTimestamp = default(string);
            var message = default(string);
            var type = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("count", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    count = reader.ReadValueAsInt();
                }
                else if (string.Compare("firstTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    firstTimestamp = reader.ReadValueAsString();
                }
                else if (string.Compare("lastTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastTimestamp = reader.ReadValueAsString();
                }
                else if (string.Compare("message", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    message = reader.ReadValueAsString();
                }
                else if (string.Compare("type", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    type = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ContainerEvent(
                name: name,
                count: count,
                firstTimestamp: firstTimestamp,
                lastTimestamp: lastTimestamp,
                message: message,
                type: type);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ContainerEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Count != null)
            {
                writer.WriteProperty(obj.Count, "count", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.FirstTimestamp != null)
            {
                writer.WriteProperty(obj.FirstTimestamp, "firstTimestamp", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LastTimestamp != null)
            {
                writer.WriteProperty(obj.LastTimestamp, "lastTimestamp", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Message != null)
            {
                writer.WriteProperty(obj.Message, "message", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Type != null)
            {
                writer.WriteProperty(obj.Type, "type", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
