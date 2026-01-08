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
    /// Converter for <see cref="SafetyCheckKind" />.
    /// </summary>
    internal class SafetyCheckKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static SafetyCheckKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(SafetyCheckKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SafetyCheckKind.Invalid;
            }
            else if (string.Compare(value, "EnsureSeedNodeQuorum", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SafetyCheckKind.EnsureSeedNodeQuorum;
            }
            else if (string.Compare(value, "EnsurePartitionQuorum", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SafetyCheckKind.EnsurePartitionQuorum;
            }
            else if (string.Compare(value, "WaitForPrimaryPlacement", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SafetyCheckKind.WaitForPrimaryPlacement;
            }
            else if (string.Compare(value, "WaitForPrimarySwap", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SafetyCheckKind.WaitForPrimarySwap;
            }
            else if (string.Compare(value, "WaitForReconfiguration", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SafetyCheckKind.WaitForReconfiguration;
            }
            else if (string.Compare(value, "WaitForInbuildReplica", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SafetyCheckKind.WaitForInbuildReplica;
            }
            else if (string.Compare(value, "EnsureAvailability", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SafetyCheckKind.EnsureAvailability;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, SafetyCheckKind? value)
        {
            switch (value)
            {
                case SafetyCheckKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case SafetyCheckKind.EnsureSeedNodeQuorum:
                    writer.WriteStringValue("EnsureSeedNodeQuorum");
                    break;
                case SafetyCheckKind.EnsurePartitionQuorum:
                    writer.WriteStringValue("EnsurePartitionQuorum");
                    break;
                case SafetyCheckKind.WaitForPrimaryPlacement:
                    writer.WriteStringValue("WaitForPrimaryPlacement");
                    break;
                case SafetyCheckKind.WaitForPrimarySwap:
                    writer.WriteStringValue("WaitForPrimarySwap");
                    break;
                case SafetyCheckKind.WaitForReconfiguration:
                    writer.WriteStringValue("WaitForReconfiguration");
                    break;
                case SafetyCheckKind.WaitForInbuildReplica:
                    writer.WriteStringValue("WaitForInbuildReplica");
                    break;
                case SafetyCheckKind.EnsureAvailability:
                    writer.WriteStringValue("EnsureAvailability");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type SafetyCheckKind");
            }
        }
    }
}
