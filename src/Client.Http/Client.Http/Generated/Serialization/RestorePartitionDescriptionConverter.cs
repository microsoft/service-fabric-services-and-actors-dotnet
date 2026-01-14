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
    /// Converter for <see cref="RestorePartitionDescription" />.
    /// </summary>
    internal class RestorePartitionDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RestorePartitionDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RestorePartitionDescription GetFromJsonProperties(JsonReader reader)
        {
            var backupId = default(Guid?);
            var backupLocation = default(string);
            var backupStorage = default(BackupStorageDescription);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("BackupId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("BackupLocation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupLocation = reader.ReadValueAsString();
                }
                else if (string.Compare("BackupStorage", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupStorage = BackupStorageDescriptionConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RestorePartitionDescription(
                backupId: backupId,
                backupLocation: backupLocation,
                backupStorage: backupStorage);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RestorePartitionDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.BackupId, "BackupId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.BackupLocation, "BackupLocation", JsonWriterExtensions.WriteStringValue);
            if (obj.BackupStorage != null)
            {
                writer.WriteProperty(obj.BackupStorage, "BackupStorage", BackupStorageDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
