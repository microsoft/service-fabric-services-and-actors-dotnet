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
    /// Converter for <see cref="BackupValidationResult" />.
    /// </summary>
    internal class BackupValidationResultConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static BackupValidationResult? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(BackupValidationResult);

            if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupValidationResult.None;
            }
            else if (string.Compare(value, "Success", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupValidationResult.Success;
            }
            else if (string.Compare(value, "ChecksumMismatchFailure", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupValidationResult.ChecksumMismatchFailure;
            }
            else if (string.Compare(value, "BackupChainMissingFailure", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupValidationResult.BackupChainMissingFailure;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, BackupValidationResult? value)
        {
            switch (value)
            {
                case BackupValidationResult.None:
                    writer.WriteStringValue("None");
                    break;
                case BackupValidationResult.Success:
                    writer.WriteStringValue("Success");
                    break;
                case BackupValidationResult.ChecksumMismatchFailure:
                    writer.WriteStringValue("ChecksumMismatchFailure");
                    break;
                case BackupValidationResult.BackupChainMissingFailure:
                    writer.WriteStringValue("BackupChainMissingFailure");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type BackupValidationResult");
            }
        }
    }
}
