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
    /// Converter for <see cref="AzureBlobBackupStorageDescription" />.
    /// </summary>
    internal class AzureBlobBackupStorageDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AzureBlobBackupStorageDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AzureBlobBackupStorageDescription GetFromJsonProperties(JsonReader reader)
        {
            var friendlyName = default(string);
            var connectionString = default(string);
            var containerName = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("FriendlyName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    friendlyName = reader.ReadValueAsString();
                }
                else if (string.Compare("ConnectionString", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    connectionString = reader.ReadValueAsString();
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

            return new AzureBlobBackupStorageDescription(
                friendlyName: friendlyName,
                connectionString: connectionString,
                containerName: containerName);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AzureBlobBackupStorageDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.StorageKind, "StorageKind", BackupStorageKindConverter.Serialize);
            writer.WriteProperty(obj.ConnectionString, "ConnectionString", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ContainerName, "ContainerName", JsonWriterExtensions.WriteStringValue);
            if (obj.FriendlyName != null)
            {
                writer.WriteProperty(obj.FriendlyName, "FriendlyName", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
