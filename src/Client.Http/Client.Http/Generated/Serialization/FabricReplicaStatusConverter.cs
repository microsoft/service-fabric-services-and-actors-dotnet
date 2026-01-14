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
    /// Converter for <see cref="FabricReplicaStatus" />.
    /// </summary>
    internal class FabricReplicaStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static FabricReplicaStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(FabricReplicaStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricReplicaStatus.Invalid;
            }
            else if (string.Compare(value, "Down", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricReplicaStatus.Down;
            }
            else if (string.Compare(value, "Up", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricReplicaStatus.Up;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, FabricReplicaStatus? value)
        {
            switch (value)
            {
                case FabricReplicaStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case FabricReplicaStatus.Down:
                    writer.WriteStringValue("Down");
                    break;
                case FabricReplicaStatus.Up:
                    writer.WriteStringValue("Up");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type FabricReplicaStatus");
            }
        }
    }
}
