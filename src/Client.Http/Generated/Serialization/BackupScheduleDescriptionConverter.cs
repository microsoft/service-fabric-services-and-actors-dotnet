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
    /// Converter for <see cref="BackupScheduleDescription" />.
    /// </summary>
    internal class BackupScheduleDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BackupScheduleDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BackupScheduleDescription GetFromJsonProperties(JsonReader reader)
        {
            BackupScheduleDescription obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("ScheduleKind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is ScheduleKind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("FrequencyBased", StringComparison.OrdinalIgnoreCase))
            {
                obj = FrequencyBasedBackupScheduleDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("TimeBased", StringComparison.OrdinalIgnoreCase))
            {
                obj = TimeBasedBackupScheduleDescriptionConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown ScheduleKind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, BackupScheduleDescription obj)
        {
            var kind = obj.ScheduleKind;
            if (kind.Equals(BackupScheduleKind.FrequencyBased))
            {
                FrequencyBasedBackupScheduleDescriptionConverter.Serialize(writer, (FrequencyBasedBackupScheduleDescription)obj);
            }
            else if (kind.Equals(BackupScheduleKind.TimeBased))
            {
                TimeBasedBackupScheduleDescriptionConverter.Serialize(writer, (TimeBasedBackupScheduleDescription)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown ScheduleKind.");
            }
        }
    }
}
