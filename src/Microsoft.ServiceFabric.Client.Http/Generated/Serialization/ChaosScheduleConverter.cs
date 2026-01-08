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
    /// Converter for <see cref="ChaosSchedule" />.
    /// </summary>
    internal class ChaosScheduleConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosSchedule Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosSchedule GetFromJsonProperties(JsonReader reader)
        {
            var startDate = default(DateTime?);
            var expiryDate = default(DateTime?);
            var chaosParametersDictionary = default(IEnumerable<ChaosParametersDictionaryItem>);
            var jobs = default(IEnumerable<ChaosScheduleJob>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("StartDate", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    startDate = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ExpiryDate", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    expiryDate = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ChaosParametersDictionary", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    chaosParametersDictionary = reader.ReadList(ChaosParametersDictionaryItemConverter.Deserialize);
                }
                else if (string.Compare("Jobs", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    jobs = reader.ReadList(ChaosScheduleJobConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ChaosSchedule(
                startDate: startDate,
                expiryDate: expiryDate,
                chaosParametersDictionary: chaosParametersDictionary,
                jobs: jobs);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ChaosSchedule obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.StartDate != null)
            {
                writer.WriteProperty(obj.StartDate, "StartDate", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ExpiryDate != null)
            {
                writer.WriteProperty(obj.ExpiryDate, "ExpiryDate", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ChaosParametersDictionary != null)
            {
                writer.WriteEnumerableProperty(obj.ChaosParametersDictionary, "ChaosParametersDictionary", ChaosParametersDictionaryItemConverter.Serialize);
            }

            if (obj.Jobs != null)
            {
                writer.WriteEnumerableProperty(obj.Jobs, "Jobs", ChaosScheduleJobConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
