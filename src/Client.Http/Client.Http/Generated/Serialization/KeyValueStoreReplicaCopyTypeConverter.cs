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
    /// Converter for <see cref="KeyValueStoreReplicaCopyType" />.
    /// </summary>
    internal class KeyValueStoreReplicaCopyTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static KeyValueStoreReplicaCopyType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(KeyValueStoreReplicaCopyType);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyType.Unknown;
            }
            else if (string.Compare(value, "Full", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyType.Full;
            }
            else if (string.Compare(value, "Partial", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreReplicaCopyType.Partial;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, KeyValueStoreReplicaCopyType? value)
        {
            switch (value)
            {
                case KeyValueStoreReplicaCopyType.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case KeyValueStoreReplicaCopyType.Full:
                    writer.WriteStringValue("Full");
                    break;
                case KeyValueStoreReplicaCopyType.Partial:
                    writer.WriteStringValue("Partial");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type KeyValueStoreReplicaCopyType");
            }
        }
    }
}
