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
    /// Converter for <see cref="BackupStorageDescription" />.
    /// </summary>
    internal class BackupStorageDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BackupStorageDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BackupStorageDescription GetFromJsonProperties(JsonReader reader)
        {
            BackupStorageDescription obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("StorageKind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is StorageKind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("AzureBlobStore", StringComparison.OrdinalIgnoreCase))
            {
                obj = AzureBlobBackupStorageDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("FileShare", StringComparison.OrdinalIgnoreCase))
            {
                obj = FileShareBackupStorageDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("DsmsAzureBlobStore", StringComparison.OrdinalIgnoreCase))
            {
                obj = DsmsAzureBlobBackupStorageDescriptionConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("ManagedIdentityAzureBlobStore", StringComparison.OrdinalIgnoreCase))
            {
                obj = ManagedIdentityAzureBlobBackupStorageDescriptionConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown StorageKind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, BackupStorageDescription obj)
        {
            var kind = obj.StorageKind;
            if (kind.Equals(BackupStorageKind.AzureBlobStore))
            {
                AzureBlobBackupStorageDescriptionConverter.Serialize(writer, (AzureBlobBackupStorageDescription)obj);
            }
            else if (kind.Equals(BackupStorageKind.FileShare))
            {
                FileShareBackupStorageDescriptionConverter.Serialize(writer, (FileShareBackupStorageDescription)obj);
            }
            else if (kind.Equals(BackupStorageKind.DsmsAzureBlobStore))
            {
                DsmsAzureBlobBackupStorageDescriptionConverter.Serialize(writer, (DsmsAzureBlobBackupStorageDescription)obj);
            }
            else if (kind.Equals(BackupStorageKind.ManagedIdentityAzureBlobStore))
            {
                ManagedIdentityAzureBlobBackupStorageDescriptionConverter.Serialize(writer, (ManagedIdentityAzureBlobBackupStorageDescription)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown StorageKind.");
            }
        }
    }
}
