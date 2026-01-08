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
    /// Converter for <see cref="TimeOfDay" />.
    /// </summary>
    internal class TimeOfDayConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static TimeOfDay Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static TimeOfDay GetFromJsonProperties(JsonReader reader)
        {
            var hour = default(int?);
            var minute = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Hour", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hour = reader.ReadValueAsInt();
                }
                else if (string.Compare("Minute", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minute = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new TimeOfDay(
                hour: hour,
                minute: minute);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, TimeOfDay obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Hour != null)
            {
                writer.WriteProperty(obj.Hour, "Hour", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.Minute != null)
            {
                writer.WriteProperty(obj.Minute, "Minute", JsonWriterExtensions.WriteIntValue);
            }

            writer.WriteEndObject();
        }
    }
}
