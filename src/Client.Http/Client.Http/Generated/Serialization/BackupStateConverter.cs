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
    /// Converter for <see cref="BackupState" />.
    /// </summary>
    internal class BackupStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static BackupState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(BackupState);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupState.Invalid;
            }
            else if (string.Compare(value, "Accepted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupState.Accepted;
            }
            else if (string.Compare(value, "BackupInProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupState.BackupInProgress;
            }
            else if (string.Compare(value, "Success", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupState.Success;
            }
            else if (string.Compare(value, "Failure", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupState.Failure;
            }
            else if (string.Compare(value, "Timeout", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupState.Timeout;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, BackupState? value)
        {
            switch (value)
            {
                case BackupState.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case BackupState.Accepted:
                    writer.WriteStringValue("Accepted");
                    break;
                case BackupState.BackupInProgress:
                    writer.WriteStringValue("BackupInProgress");
                    break;
                case BackupState.Success:
                    writer.WriteStringValue("Success");
                    break;
                case BackupState.Failure:
                    writer.WriteStringValue("Failure");
                    break;
                case BackupState.Timeout:
                    writer.WriteStringValue("Timeout");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type BackupState");
            }
        }
    }
}
