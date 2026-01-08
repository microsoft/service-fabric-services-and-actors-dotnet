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
    /// Converter for <see cref="FileShareBackupStorageDescription" />.
    /// </summary>
    internal class FileShareBackupStorageDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static FileShareBackupStorageDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static FileShareBackupStorageDescription GetFromJsonProperties(JsonReader reader)
        {
            var friendlyName = default(string);
            var path = default(string);
            var primaryUserName = default(string);
            var primaryPassword = default(string);
            var secondaryUserName = default(string);
            var secondaryPassword = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("FriendlyName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    friendlyName = reader.ReadValueAsString();
                }
                else if (string.Compare("Path", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    path = reader.ReadValueAsString();
                }
                else if (string.Compare("PrimaryUserName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryUserName = reader.ReadValueAsString();
                }
                else if (string.Compare("PrimaryPassword", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryPassword = reader.ReadValueAsString();
                }
                else if (string.Compare("SecondaryUserName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryUserName = reader.ReadValueAsString();
                }
                else if (string.Compare("SecondaryPassword", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryPassword = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new FileShareBackupStorageDescription(
                friendlyName: friendlyName,
                path: path,
                primaryUserName: primaryUserName,
                primaryPassword: primaryPassword,
                secondaryUserName: secondaryUserName,
                secondaryPassword: secondaryPassword);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, FileShareBackupStorageDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.StorageKind, "StorageKind", BackupStorageKindConverter.Serialize);
            writer.WriteProperty(obj.Path, "Path", JsonWriterExtensions.WriteStringValue);
            if (obj.FriendlyName != null)
            {
                writer.WriteProperty(obj.FriendlyName, "FriendlyName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PrimaryUserName != null)
            {
                writer.WriteProperty(obj.PrimaryUserName, "PrimaryUserName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PrimaryPassword != null)
            {
                writer.WriteProperty(obj.PrimaryPassword, "PrimaryPassword", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.SecondaryUserName != null)
            {
                writer.WriteProperty(obj.SecondaryUserName, "SecondaryUserName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.SecondaryPassword != null)
            {
                writer.WriteProperty(obj.SecondaryPassword, "SecondaryPassword", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
