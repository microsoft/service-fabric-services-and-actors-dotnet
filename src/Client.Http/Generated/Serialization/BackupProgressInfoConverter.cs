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
    /// Converter for <see cref="BackupProgressInfo" />.
    /// </summary>
    internal class BackupProgressInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BackupProgressInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static BackupProgressInfo GetFromJsonProperties(JsonReader reader)
        {
            var backupState = default(BackupState?);
            var timeStampUtc = default(DateTime?);
            var backupId = default(Guid?);
            var backupLocation = default(string);
            var epochOfLastBackupRecord = default(Epoch);
            var lsnOfLastBackupRecord = default(string);
            var failureError = default(FabricErrorError);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("BackupState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupState = BackupStateConverter.Deserialize(reader);
                }
                else if (string.Compare("TimeStampUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeStampUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("BackupId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("BackupLocation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    backupLocation = reader.ReadValueAsString();
                }
                else if (string.Compare("EpochOfLastBackupRecord", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    epochOfLastBackupRecord = EpochConverter.Deserialize(reader);
                }
                else if (string.Compare("LsnOfLastBackupRecord", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lsnOfLastBackupRecord = reader.ReadValueAsString();
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

            return new BackupProgressInfo(
                backupState: backupState,
                timeStampUtc: timeStampUtc,
                backupId: backupId,
                backupLocation: backupLocation,
                epochOfLastBackupRecord: epochOfLastBackupRecord,
                lsnOfLastBackupRecord: lsnOfLastBackupRecord,
                failureError: failureError);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, BackupProgressInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.BackupState, "BackupState", BackupStateConverter.Serialize);
            if (obj.TimeStampUtc != null)
            {
                writer.WriteProperty(obj.TimeStampUtc, "TimeStampUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.BackupId != null)
            {
                writer.WriteProperty(obj.BackupId, "BackupId", JsonWriterExtensions.WriteGuidValue);
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

            if (obj.FailureError != null)
            {
                writer.WriteProperty(obj.FailureError, "FailureError", FabricErrorErrorConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
