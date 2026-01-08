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
    /// Converter for <see cref="TimeBasedBackupScheduleDescription" />.
    /// </summary>
    internal class TimeBasedBackupScheduleDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static TimeBasedBackupScheduleDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static TimeBasedBackupScheduleDescription GetFromJsonProperties(JsonReader reader)
        {
            var scheduleFrequencyType = default(BackupScheduleFrequencyType?);
            var runDays = default(IEnumerable<DayOfWeek?>);
            var runTimes = default(IEnumerable<DateTime?>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ScheduleFrequencyType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scheduleFrequencyType = BackupScheduleFrequencyTypeConverter.Deserialize(reader);
                }
                else if (string.Compare("RunDays", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    runDays = reader.ReadList(DayOfWeekConverter.Deserialize);
                }
                else if (string.Compare("RunTimes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    runTimes = reader.ReadList(JsonReaderExtensions.ReadValueAsDateTime);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new TimeBasedBackupScheduleDescription(
                scheduleFrequencyType: scheduleFrequencyType,
                runDays: runDays,
                runTimes: runTimes);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, TimeBasedBackupScheduleDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ScheduleKind, "ScheduleKind", BackupScheduleKindConverter.Serialize);
            writer.WriteProperty(obj.ScheduleFrequencyType, "ScheduleFrequencyType", BackupScheduleFrequencyTypeConverter.Serialize);
            writer.WriteEnumerableProperty(obj.RunTimes, "RunTimes", (w, v) => writer.WriteDateTimeValue(v));
            if (obj.RunDays != null)
            {
                writer.WriteEnumerableProperty(obj.RunDays, "RunDays", DayOfWeekConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
