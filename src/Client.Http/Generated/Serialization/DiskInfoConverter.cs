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
    /// Converter for <see cref="DiskInfo" />.
    /// </summary>
    internal class DiskInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DiskInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DiskInfo GetFromJsonProperties(JsonReader reader)
        {
            var capacity = default(string);
            var availableSpace = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Capacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    capacity = reader.ReadValueAsString();
                }
                else if (string.Compare("AvailableSpace", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    availableSpace = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DiskInfo(
                capacity: capacity,
                availableSpace: availableSpace);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DiskInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Capacity != null)
            {
                writer.WriteProperty(obj.Capacity, "Capacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.AvailableSpace != null)
            {
                writer.WriteProperty(obj.AvailableSpace, "AvailableSpace", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
