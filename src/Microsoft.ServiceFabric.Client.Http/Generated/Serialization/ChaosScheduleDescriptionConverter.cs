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
    /// Converter for <see cref="ChaosScheduleDescription" />.
    /// </summary>
    internal class ChaosScheduleDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosScheduleDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosScheduleDescription GetFromJsonProperties(JsonReader reader)
        {
            var version = default(int?);
            var schedule = default(ChaosSchedule);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Version", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    version = reader.ReadValueAsInt();
                }
                else if (string.Compare("Schedule", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    schedule = ChaosScheduleConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ChaosScheduleDescription(
                version: version,
                schedule: schedule);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ChaosScheduleDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Version != null)
            {
                writer.WriteProperty(obj.Version, "Version", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.Schedule != null)
            {
                writer.WriteProperty(obj.Schedule, "Schedule", ChaosScheduleConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
