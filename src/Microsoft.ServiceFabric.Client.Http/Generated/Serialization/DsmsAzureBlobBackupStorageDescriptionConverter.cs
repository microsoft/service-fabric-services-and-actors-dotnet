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
    /// Converter for <see cref="DsmsAzureBlobBackupStorageDescription" />.
    /// </summary>
    internal class DsmsAzureBlobBackupStorageDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DsmsAzureBlobBackupStorageDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DsmsAzureBlobBackupStorageDescription GetFromJsonProperties(JsonReader reader)
        {
            var friendlyName = default(string);
            var storageCredentialsSourceLocation = default(string);
            var containerName = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("FriendlyName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    friendlyName = reader.ReadValueAsString();
                }
                else if (string.Compare("StorageCredentialsSourceLocation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    storageCredentialsSourceLocation = reader.ReadValueAsString();
                }
                else if (string.Compare("ContainerName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    containerName = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DsmsAzureBlobBackupStorageDescription(
                friendlyName: friendlyName,
                storageCredentialsSourceLocation: storageCredentialsSourceLocation,
                containerName: containerName);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DsmsAzureBlobBackupStorageDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.StorageKind, "StorageKind", BackupStorageKindConverter.Serialize);
            writer.WriteProperty(obj.StorageCredentialsSourceLocation, "StorageCredentialsSourceLocation", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ContainerName, "ContainerName", JsonWriterExtensions.WriteStringValue);
            if (obj.FriendlyName != null)
            {
                writer.WriteProperty(obj.FriendlyName, "FriendlyName", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
