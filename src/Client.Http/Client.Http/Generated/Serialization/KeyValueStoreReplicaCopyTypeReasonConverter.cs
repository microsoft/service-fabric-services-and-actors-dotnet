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
    /// Converter for <see cref="KeyValueStoreReplicaCopyTypeReason" />.
    /// </summary>
    internal class KeyValueStoreReplicaCopyTypeReasonConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static KeyValueStoreReplicaCopyTypeReason? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(KeyValueStoreReplicaCopyTypeReason);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyTypeReason.Unknown;
            }
            else if (string.Compare(value, "InvalidSecondaryEpoch", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyTypeReason.InvalidSecondaryEpoch;
            }
            else if (string.Compare(value, "EmptySecondary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyTypeReason.EmptySecondary;
            }
            else if (string.Compare(value, "FalseProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyTypeReason.FalseProgress;
            }
            else if (string.Compare(value, "MatchedConfigurationNumber", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyTypeReason.MatchedConfigurationNumber;
            }
            else if (string.Compare(value, "EpochNotFound", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyTypeReason.EpochNotFound;
            }
            else if (string.Compare(value, "StaleSecondary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyTypeReason.StaleSecondary;
            }
            else if (string.Compare(value, "PrimaryTombstonesNotTruncated", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyTypeReason.PrimaryTombstonesNotTruncated;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, KeyValueStoreReplicaCopyTypeReason? value)
        {
            switch (value)
            {
                case KeyValueStoreReplicaCopyTypeReason.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case KeyValueStoreReplicaCopyTypeReason.InvalidSecondaryEpoch:
                    writer.WriteStringValue("InvalidSecondaryEpoch");
                    break;
                case KeyValueStoreReplicaCopyTypeReason.EmptySecondary:
                    writer.WriteStringValue("EmptySecondary");
                    break;
                case KeyValueStoreReplicaCopyTypeReason.FalseProgress:
                    writer.WriteStringValue("FalseProgress");
                    break;
                case KeyValueStoreReplicaCopyTypeReason.MatchedConfigurationNumber:
                    writer.WriteStringValue("MatchedConfigurationNumber");
                    break;
                case KeyValueStoreReplicaCopyTypeReason.EpochNotFound:
                    writer.WriteStringValue("EpochNotFound");
                    break;
                case KeyValueStoreReplicaCopyTypeReason.StaleSecondary:
                    writer.WriteStringValue("StaleSecondary");
                    break;
                case KeyValueStoreReplicaCopyTypeReason.PrimaryTombstonesNotTruncated:
                    writer.WriteStringValue("PrimaryTombstonesNotTruncated");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type KeyValueStoreReplicaCopyTypeReason");
            }
        }
    }
}
