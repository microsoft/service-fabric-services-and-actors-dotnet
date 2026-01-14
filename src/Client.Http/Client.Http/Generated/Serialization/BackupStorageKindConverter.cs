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
    /// Converter for <see cref="BackupStorageKind" />.
    /// </summary>
    internal class BackupStorageKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static BackupStorageKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(BackupStorageKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupStorageKind.Invalid;
            }
            else if (string.Compare(value, "FileShare", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupStorageKind.FileShare;
            }
            else if (string.Compare(value, "AzureBlobStore", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupStorageKind.AzureBlobStore;
            }
            else if (string.Compare(value, "DsmsAzureBlobStore", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupStorageKind.DsmsAzureBlobStore;
            }
            else if (string.Compare(value, "ManagedIdentityAzureBlobStore", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupStorageKind.ManagedIdentityAzureBlobStore;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, BackupStorageKind? value)
        {
            switch (value)
            {
                case BackupStorageKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case BackupStorageKind.FileShare:
                    writer.WriteStringValue("FileShare");
                    break;
                case BackupStorageKind.AzureBlobStore:
                    writer.WriteStringValue("AzureBlobStore");
                    break;
                case BackupStorageKind.DsmsAzureBlobStore:
                    writer.WriteStringValue("DsmsAzureBlobStore");
                    break;
                case BackupStorageKind.ManagedIdentityAzureBlobStore:
                    writer.WriteStringValue("ManagedIdentityAzureBlobStore");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type BackupStorageKind");
            }
        }
    }
}
