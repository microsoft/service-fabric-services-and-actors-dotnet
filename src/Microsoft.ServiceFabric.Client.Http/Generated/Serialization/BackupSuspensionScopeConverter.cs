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
    /// Converter for <see cref="BackupSuspensionScope" />.
    /// </summary>
    internal class BackupSuspensionScopeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static BackupSuspensionScope? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(BackupSuspensionScope);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupSuspensionScope.Invalid;
            }
            else if (string.Compare(value, "Partition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupSuspensionScope.Partition;
            }
            else if (string.Compare(value, "Service", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupSuspensionScope.Service;
            }
            else if (string.Compare(value, "Application", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupSuspensionScope.Application;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, BackupSuspensionScope? value)
        {
            switch (value)
            {
                case BackupSuspensionScope.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case BackupSuspensionScope.Partition:
                    writer.WriteStringValue("Partition");
                    break;
                case BackupSuspensionScope.Service:
                    writer.WriteStringValue("Service");
                    break;
                case BackupSuspensionScope.Application:
                    writer.WriteStringValue("Application");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type BackupSuspensionScope");
            }
        }
    }
}
