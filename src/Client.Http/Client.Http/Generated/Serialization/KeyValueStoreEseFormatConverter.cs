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
    /// Converter for <see cref="KeyValueStoreEseFormat" />.
    /// </summary>
    internal class KeyValueStoreEseFormatConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static KeyValueStoreEseFormat? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(KeyValueStoreEseFormat);

            if (string.Compare(value, "Legacy", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreEseFormat.Legacy;
            }
            else if (string.Compare(value, "Hop1", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreEseFormat.Hop1;
            }
            else if (string.Compare(value, "Hop2Legacy", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KeyValueStoreEseFormat.Hop2Legacy;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, KeyValueStoreEseFormat? value)
        {
            switch (value)
            {
                case KeyValueStoreEseFormat.Legacy:
                    writer.WriteStringValue("Legacy");
                    break;
                case KeyValueStoreEseFormat.Hop1:
                    writer.WriteStringValue("Hop1");
                    break;
                case KeyValueStoreEseFormat.Hop2Legacy:
                    writer.WriteStringValue("Hop2Legacy");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type KeyValueStoreEseFormat");
            }
        }
    }
}
