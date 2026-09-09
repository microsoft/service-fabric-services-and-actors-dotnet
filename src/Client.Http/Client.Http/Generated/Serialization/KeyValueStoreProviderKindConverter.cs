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
    /// Converter for <see cref="KeyValueStoreProviderKind" />.
    /// </summary>
    internal class KeyValueStoreProviderKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static KeyValueStoreProviderKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(KeyValueStoreProviderKind);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreProviderKind.Unknown;
            }
            else if (string.Compare(value, "Ese", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreProviderKind.Ese;
            }
            else if (string.Compare(value, "TStore", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreProviderKind.TStore;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, KeyValueStoreProviderKind? value)
        {
            switch (value)
            {
                case KeyValueStoreProviderKind.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case KeyValueStoreProviderKind.Ese:
                    writer.WriteStringValue("Ese");
                    break;
                case KeyValueStoreProviderKind.TStore:
                    writer.WriteStringValue("TStore");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type KeyValueStoreProviderKind");
            }
        }
    }
}
