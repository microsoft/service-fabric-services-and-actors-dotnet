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
    /// Converter for <see cref="KvsReplicaCopyMode" />.
    /// </summary>
    internal class KvsReplicaCopyModeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static KvsReplicaCopyMode? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(KvsReplicaCopyMode);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyMode.Unknown;
            }
            else if (string.Compare(value, "Physical", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyMode.Physical;
            }
            else if (string.Compare(value, "Logical", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = KvsReplicaCopyMode.Logical;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, KvsReplicaCopyMode? value)
        {
            switch (value)
            {
                case KvsReplicaCopyMode.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case KvsReplicaCopyMode.Physical:
                    writer.WriteStringValue("Physical");
                    break;
                case KvsReplicaCopyMode.Logical:
                    writer.WriteStringValue("Logical");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type KvsReplicaCopyMode");
            }
        }
    }
}
