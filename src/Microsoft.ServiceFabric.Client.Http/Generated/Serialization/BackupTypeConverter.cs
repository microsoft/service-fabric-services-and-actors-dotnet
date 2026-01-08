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
    /// Converter for <see cref="BackupType" />.
    /// </summary>
    internal class BackupTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static BackupType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(BackupType);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupType.Invalid;
            }
            else if (string.Compare(value, "Full", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupType.Full;
            }
            else if (string.Compare(value, "Incremental", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupType.Incremental;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, BackupType? value)
        {
            switch (value)
            {
                case BackupType.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case BackupType.Full:
                    writer.WriteStringValue("Full");
                    break;
                case BackupType.Incremental:
                    writer.WriteStringValue("Incremental");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type BackupType");
            }
        }
    }
}
