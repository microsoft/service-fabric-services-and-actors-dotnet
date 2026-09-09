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
    /// Converter for <see cref="KeyValueStoreReplicaCopyModeReason" />.
    /// </summary>
    internal class KeyValueStoreReplicaCopyModeReasonConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static KeyValueStoreReplicaCopyModeReason? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(KeyValueStoreReplicaCopyModeReason);

            if (string.Compare(value, "DefaultPhysicalCopy", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyModeReason.DefaultPhysicalCopy;
            }
            else if (string.Compare(value, "LogicalCopyProbability", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyModeReason.LogicalCopyProbability;
            }
            else if (string.Compare(value, "IncompatibleStoreFormatVersion", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyModeReason.IncompatibleStoreFormatVersion;
            }
            else if (string.Compare(value, "EmptyDatabase", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyModeReason.EmptyDatabase;
            }
            else if (string.Compare(value, "FileStreamFullCopyNotSupportedBySecondary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyModeReason.FileStreamFullCopyNotSupportedBySecondary;
            }
            else if (string.Compare(value, "FullCopyModeConfiguredAsLogical", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyModeReason.FullCopyModeConfiguredAsLogical;
            }
            else if (string.Compare(value, "EnableFileStreamFullCopySetToFalse", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyModeReason.EnableFileStreamFullCopySetToFalse;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, KeyValueStoreReplicaCopyModeReason? value)
        {
            switch (value)
            {
                case KeyValueStoreReplicaCopyModeReason.DefaultPhysicalCopy:
                    writer.WriteStringValue("DefaultPhysicalCopy");
                    break;
                case KeyValueStoreReplicaCopyModeReason.LogicalCopyProbability:
                    writer.WriteStringValue("LogicalCopyProbability");
                    break;
                case KeyValueStoreReplicaCopyModeReason.IncompatibleStoreFormatVersion:
                    writer.WriteStringValue("IncompatibleStoreFormatVersion");
                    break;
                case KeyValueStoreReplicaCopyModeReason.EmptyDatabase:
                    writer.WriteStringValue("EmptyDatabase");
                    break;
                case KeyValueStoreReplicaCopyModeReason.FileStreamFullCopyNotSupportedBySecondary:
                    writer.WriteStringValue("FileStreamFullCopyNotSupportedBySecondary");
                    break;
                case KeyValueStoreReplicaCopyModeReason.FullCopyModeConfiguredAsLogical:
                    writer.WriteStringValue("FullCopyModeConfiguredAsLogical");
                    break;
                case KeyValueStoreReplicaCopyModeReason.EnableFileStreamFullCopySetToFalse:
                    writer.WriteStringValue("EnableFileStreamFullCopySetToFalse");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type KeyValueStoreReplicaCopyModeReason");
            }
        }
    }
}
