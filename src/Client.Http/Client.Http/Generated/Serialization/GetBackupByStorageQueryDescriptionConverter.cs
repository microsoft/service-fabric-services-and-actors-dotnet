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
    /// Converter for <see cref="GetBackupByStorageQueryDescription" />.
    /// </summary>
    internal class GetBackupByStorageQueryDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static GetBackupByStorageQueryDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static GetBackupByStorageQueryDescription GetFromJsonProperties(JsonReader reader)
        {
            var startDateTimeFilter = default(DateTime?);
            var endDateTimeFilter = default(DateTime?);
            var latest = default(bool?);
            var storage = default(BackupStorageDescription);
            var backupEntity = default(BackupEntity);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("StartDateTimeFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    startDateTimeFilter = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("EndDateTimeFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    endDateTimeFilter = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("Latest", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    latest = reader.ReadValueAsBool();
                }
                else if (string.Compare("Storage", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    storage = BackupStorageDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("BackupEntity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupEntity = BackupEntityConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new GetBackupByStorageQueryDescription(
                startDateTimeFilter: startDateTimeFilter,
                endDateTimeFilter: endDateTimeFilter,
                latest: latest,
                storage: storage,
                backupEntity: backupEntity);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, GetBackupByStorageQueryDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Storage, "Storage", BackupStorageDescriptionConverter.Serialize);
            writer.WriteProperty(obj.BackupEntity, "BackupEntity", BackupEntityConverter.Serialize);
            if (obj.StartDateTimeFilter != null)
            {
                writer.WriteProperty(obj.StartDateTimeFilter, "StartDateTimeFilter", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.EndDateTimeFilter != null)
            {
                writer.WriteProperty(obj.EndDateTimeFilter, "EndDateTimeFilter", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.Latest != null)
            {
                writer.WriteProperty(obj.Latest, "Latest", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
