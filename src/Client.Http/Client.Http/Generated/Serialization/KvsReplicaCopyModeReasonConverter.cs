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
    /// Converter for <see cref="KvsReplicaCopyModeReason" />.
    /// </summary>
    internal class KvsReplicaCopyModeReasonConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static KvsReplicaCopyModeReason? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(KvsReplicaCopyModeReason);

            if (string.Compare(value, "DefaultPhysicalCopy", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyModeReason.DefaultPhysicalCopy;
            }
            else if (string.Compare(value, "LogicalCopyProbability", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyModeReason.LogicalCopyProbability;
            }
            else if (string.Compare(value, "IncompatibleStoreFormatVersion", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyModeReason.IncompatibleStoreFormatVersion;
            }
            else if (string.Compare(value, "EmptyDatabase", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyModeReason.EmptyDatabase;
            }
            else if (string.Compare(value, "FileStreamFullCopyNotSupportedBySecondary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyModeReason.FileStreamFullCopyNotSupportedBySecondary;
            }
            else if (string.Compare(value, "FullCopyModeConfiguredAsLogical", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyModeReason.FullCopyModeConfiguredAsLogical;
            }
            else if (string.Compare(value, "EnableFileStreamFullCopySetToFalse", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyModeReason.EnableFileStreamFullCopySetToFalse;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, KvsReplicaCopyModeReason? value)
        {
            switch (value)
            {
                case KvsReplicaCopyModeReason.DefaultPhysicalCopy:
                    writer.WriteStringValue("DefaultPhysicalCopy");
                    break;
                case KvsReplicaCopyModeReason.LogicalCopyProbability:
                    writer.WriteStringValue("LogicalCopyProbability");
                    break;
                case KvsReplicaCopyModeReason.IncompatibleStoreFormatVersion:
                    writer.WriteStringValue("IncompatibleStoreFormatVersion");
                    break;
                case KvsReplicaCopyModeReason.EmptyDatabase:
                    writer.WriteStringValue("EmptyDatabase");
                    break;
                case KvsReplicaCopyModeReason.FileStreamFullCopyNotSupportedBySecondary:
                    writer.WriteStringValue("FileStreamFullCopyNotSupportedBySecondary");
                    break;
                case KvsReplicaCopyModeReason.FullCopyModeConfiguredAsLogical:
                    writer.WriteStringValue("FullCopyModeConfiguredAsLogical");
                    break;
                case KvsReplicaCopyModeReason.EnableFileStreamFullCopySetToFalse:
                    writer.WriteStringValue("EnableFileStreamFullCopySetToFalse");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type KvsReplicaCopyModeReason");
            }
        }
    }
}
