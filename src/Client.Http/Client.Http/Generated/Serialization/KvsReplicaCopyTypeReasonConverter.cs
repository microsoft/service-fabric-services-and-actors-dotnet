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
    /// Converter for <see cref="KvsReplicaCopyTypeReason" />.
    /// </summary>
    internal class KvsReplicaCopyTypeReasonConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static KvsReplicaCopyTypeReason? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(KvsReplicaCopyTypeReason);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyTypeReason.Unknown;
            }
            else if (string.Compare(value, "InvalidSecondaryEpoch", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyTypeReason.InvalidSecondaryEpoch;
            }
            else if (string.Compare(value, "EmptySecondary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyTypeReason.EmptySecondary;
            }
            else if (string.Compare(value, "FalseProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyTypeReason.FalseProgress;
            }
            else if (string.Compare(value, "MatchedConfigurationNumber", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyTypeReason.MatchedConfigurationNumber;
            }
            else if (string.Compare(value, "EpochNotFound", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyTypeReason.EpochNotFound;
            }
            else if (string.Compare(value, "StaleSecondary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyTypeReason.StaleSecondary;
            }
            else if (string.Compare(value, "PrimaryTombstonesNotTruncated", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyTypeReason.PrimaryTombstonesNotTruncated;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, KvsReplicaCopyTypeReason? value)
        {
            switch (value)
            {
                case KvsReplicaCopyTypeReason.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case KvsReplicaCopyTypeReason.InvalidSecondaryEpoch:
                    writer.WriteStringValue("InvalidSecondaryEpoch");
                    break;
                case KvsReplicaCopyTypeReason.EmptySecondary:
                    writer.WriteStringValue("EmptySecondary");
                    break;
                case KvsReplicaCopyTypeReason.FalseProgress:
                    writer.WriteStringValue("FalseProgress");
                    break;
                case KvsReplicaCopyTypeReason.MatchedConfigurationNumber:
                    writer.WriteStringValue("MatchedConfigurationNumber");
                    break;
                case KvsReplicaCopyTypeReason.EpochNotFound:
                    writer.WriteStringValue("EpochNotFound");
                    break;
                case KvsReplicaCopyTypeReason.StaleSecondary:
                    writer.WriteStringValue("StaleSecondary");
                    break;
                case KvsReplicaCopyTypeReason.PrimaryTombstonesNotTruncated:
                    writer.WriteStringValue("PrimaryTombstonesNotTruncated");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type KvsReplicaCopyTypeReason");
            }
        }
    }
}
