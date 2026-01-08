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
    /// Converter for <see cref="ChaosScheduleJobActiveDaysOfWeek" />.
    /// </summary>
    internal class ChaosScheduleJobActiveDaysOfWeekConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosScheduleJobActiveDaysOfWeek Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosScheduleJobActiveDaysOfWeek GetFromJsonProperties(JsonReader reader)
        {
            var sunday = default(bool?);
            var monday = default(bool?);
            var tuesday = default(bool?);
            var wednesday = default(bool?);
            var thursday = default(bool?);
            var friday = default(bool?);
            var saturday = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Sunday", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sunday = reader.ReadValueAsBool();
                }
                else if (string.Compare("Monday", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    monday = reader.ReadValueAsBool();
                }
                else if (string.Compare("Tuesday", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tuesday = reader.ReadValueAsBool();
                }
                else if (string.Compare("Wednesday", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    wednesday = reader.ReadValueAsBool();
                }
                else if (string.Compare("Thursday", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    thursday = reader.ReadValueAsBool();
                }
                else if (string.Compare("Friday", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    friday = reader.ReadValueAsBool();
                }
                else if (string.Compare("Saturday", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    saturday = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ChaosScheduleJobActiveDaysOfWeek(
                sunday: sunday,
                monday: monday,
                tuesday: tuesday,
                wednesday: wednesday,
                thursday: thursday,
                friday: friday,
                saturday: saturday);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ChaosScheduleJobActiveDaysOfWeek obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Sunday != null)
            {
                writer.WriteProperty(obj.Sunday, "Sunday", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.Monday != null)
            {
                writer.WriteProperty(obj.Monday, "Monday", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.Tuesday != null)
            {
                writer.WriteProperty(obj.Tuesday, "Tuesday", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.Wednesday != null)
            {
                writer.WriteProperty(obj.Wednesday, "Wednesday", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.Thursday != null)
            {
                writer.WriteProperty(obj.Thursday, "Thursday", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.Friday != null)
            {
                writer.WriteProperty(obj.Friday, "Friday", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.Saturday != null)
            {
                writer.WriteProperty(obj.Saturday, "Saturday", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
