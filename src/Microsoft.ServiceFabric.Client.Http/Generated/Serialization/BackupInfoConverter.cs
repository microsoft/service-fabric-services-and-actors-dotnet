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
    /// Converter for <see cref="BackupInfo" />.
    /// </summary>
    internal class BackupInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BackupInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static BackupInfo GetFromJsonProperties(JsonReader reader)
        {
            var backupId = default(Guid?);
            var backupChainId = default(Guid?);
            var applicationName = default(string);
            var serviceName = default(string);
            var partitionInformation = default(PartitionInformation);
            var backupLocation = default(string);
            var backupType = default(BackupType?);
            var epochOfLastBackupRecord = default(Epoch);
            var lsnOfLastBackupRecord = default(string);
            var creationTimeUtc = default(DateTime?);
            var serviceManifestVersion = default(string);
            var failureError = default(FabricErrorError);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("BackupId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("BackupChainId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupChainId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("ApplicationName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = reader.ReadValueAsString();
                }
                else if (string.Compare("PartitionInformation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionInformation = PartitionInformationConverter.Deserialize(reader);
                }
                else if (string.Compare("BackupLocation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupLocation = reader.ReadValueAsString();
                }
                else if (string.Compare("BackupType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupType = BackupTypeConverter.Deserialize(reader);
                }
                else if (string.Compare("EpochOfLastBackupRecord", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    epochOfLastBackupRecord = EpochConverter.Deserialize(reader);
                }
                else if (string.Compare("LsnOfLastBackupRecord", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lsnOfLastBackupRecord = reader.ReadValueAsString();
                }
                else if (string.Compare("CreationTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    creationTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ServiceManifestVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceManifestVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("FailureError", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    failureError = FabricErrorErrorConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new BackupInfo(
                backupId: backupId,
                backupChainId: backupChainId,
                applicationName: applicationName,
                serviceName: serviceName,
                partitionInformation: partitionInformation,
                backupLocation: backupLocation,
                backupType: backupType,
                epochOfLastBackupRecord: epochOfLastBackupRecord,
                lsnOfLastBackupRecord: lsnOfLastBackupRecord,
                creationTimeUtc: creationTimeUtc,
                serviceManifestVersion: serviceManifestVersion,
                failureError: failureError);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, BackupInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.BackupType, "BackupType", BackupTypeConverter.Serialize);
            if (obj.BackupId != null)
            {
                writer.WriteProperty(obj.BackupId, "BackupId", JsonWriterExtensions.WriteGuidValue);
            }

            if (obj.BackupChainId != null)
            {
                writer.WriteProperty(obj.BackupChainId, "BackupChainId", JsonWriterExtensions.WriteGuidValue);
            }

            if (obj.ApplicationName != null)
            {
                writer.WriteProperty(obj.ApplicationName, "ApplicationName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceName != null)
            {
                writer.WriteProperty(obj.ServiceName, "ServiceName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PartitionInformation != null)
            {
                writer.WriteProperty(obj.PartitionInformation, "PartitionInformation", PartitionInformationConverter.Serialize);
            }

            if (obj.BackupLocation != null)
            {
                writer.WriteProperty(obj.BackupLocation, "BackupLocation", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.EpochOfLastBackupRecord != null)
            {
                writer.WriteProperty(obj.EpochOfLastBackupRecord, "EpochOfLastBackupRecord", EpochConverter.Serialize);
            }

            if (obj.LsnOfLastBackupRecord != null)
            {
                writer.WriteProperty(obj.LsnOfLastBackupRecord, "LsnOfLastBackupRecord", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CreationTimeUtc != null)
            {
                writer.WriteProperty(obj.CreationTimeUtc, "CreationTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ServiceManifestVersion != null)
            {
                writer.WriteProperty(obj.ServiceManifestVersion, "ServiceManifestVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FailureError != null)
            {
                writer.WriteProperty(obj.FailureError, "FailureError", FabricErrorErrorConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
