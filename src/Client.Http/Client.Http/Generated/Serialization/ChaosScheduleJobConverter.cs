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
    /// Converter for <see cref="ChaosScheduleJob" />.
    /// </summary>
    internal class ChaosScheduleJobConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosScheduleJob Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosScheduleJob GetFromJsonProperties(JsonReader reader)
        {
            var chaosParameters = default(string);
            var days = default(ChaosScheduleJobActiveDaysOfWeek);
            var times = default(IEnumerable<TimeRange>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ChaosParameters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    chaosParameters = reader.ReadValueAsString();
                }
                else if (string.Compare("Days", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    days = ChaosScheduleJobActiveDaysOfWeekConverter.Deserialize(reader);
                }
                else if (string.Compare("Times", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    times = reader.ReadList(TimeRangeConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ChaosScheduleJob(
                chaosParameters: chaosParameters,
                days: days,
                times: times);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ChaosScheduleJob obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ChaosParameters != null)
            {
                writer.WriteProperty(obj.ChaosParameters, "ChaosParameters", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Days != null)
            {
                writer.WriteProperty(obj.Days, "Days", ChaosScheduleJobActiveDaysOfWeekConverter.Serialize);
            }

            if (obj.Times != null)
            {
                writer.WriteEnumerableProperty(obj.Times, "Times", TimeRangeConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
