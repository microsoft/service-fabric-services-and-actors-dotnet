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
    /// Converter for <see cref="PartitionAccessStatus" />.
    /// </summary>
    internal class PartitionAccessStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static PartitionAccessStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(PartitionAccessStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PartitionAccessStatus.Invalid;
            }
            else if (string.Compare(value, "Granted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PartitionAccessStatus.Granted;
            }
            else if (string.Compare(value, "ReconfigurationPending", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PartitionAccessStatus.ReconfigurationPending;
            }
            else if (string.Compare(value, "NotPrimary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PartitionAccessStatus.NotPrimary;
            }
            else if (string.Compare(value, "NoWriteQuorum", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PartitionAccessStatus.NoWriteQuorum;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, PartitionAccessStatus? value)
        {
            switch (value)
            {
                case PartitionAccessStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case PartitionAccessStatus.Granted:
                    writer.WriteStringValue("Granted");
                    break;
                case PartitionAccessStatus.ReconfigurationPending:
                    writer.WriteStringValue("ReconfigurationPending");
                    break;
                case PartitionAccessStatus.NotPrimary:
                    writer.WriteStringValue("NotPrimary");
                    break;
                case PartitionAccessStatus.NoWriteQuorum:
                    writer.WriteStringValue("NoWriteQuorum");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type PartitionAccessStatus");
            }
        }
    }
}
