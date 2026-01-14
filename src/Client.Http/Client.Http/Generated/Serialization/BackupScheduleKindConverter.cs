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
    /// Converter for <see cref="BackupScheduleKind" />.
    /// </summary>
    internal class BackupScheduleKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static BackupScheduleKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(BackupScheduleKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupScheduleKind.Invalid;
            }
            else if (string.Compare(value, "TimeBased", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupScheduleKind.TimeBased;
            }
            else if (string.Compare(value, "FrequencyBased", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = BackupScheduleKind.FrequencyBased;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, BackupScheduleKind? value)
        {
            switch (value)
            {
                case BackupScheduleKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case BackupScheduleKind.TimeBased:
                    writer.WriteStringValue("TimeBased");
                    break;
                case BackupScheduleKind.FrequencyBased:
                    writer.WriteStringValue("FrequencyBased");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type BackupScheduleKind");
            }
        }
    }
}
