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
    /// Converter for <see cref="RepairImpactKind" />.
    /// </summary>
    internal class RepairImpactKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static RepairImpactKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(RepairImpactKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RepairImpactKind.Invalid;
            }
            else if (string.Compare(value, "Node", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RepairImpactKind.Node;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, RepairImpactKind? value)
        {
            switch (value)
            {
                case RepairImpactKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case RepairImpactKind.Node:
                    writer.WriteStringValue("Node");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type RepairImpactKind");
            }
        }
    }
}
