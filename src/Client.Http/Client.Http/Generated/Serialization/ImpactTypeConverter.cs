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
    /// Converter for <see cref="ImpactType" />.
    /// </summary>
    internal class ImpactTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ImpactType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ImpactType);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactType.Unknown;
            }
            else if (string.Compare(value, "NodeDeactivation", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactType.NodeDeactivation;
            }
            else if (string.Compare(value, "ApplicationUpgrade", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactType.ApplicationUpgrade;
            }
            else if (string.Compare(value, "FabricUpgrade", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactType.FabricUpgrade;
            }
            else if (string.Compare(value, "Partition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactType.Partition;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ImpactType? value)
        {
            switch (value)
            {
                case ImpactType.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case ImpactType.NodeDeactivation:
                    writer.WriteStringValue("NodeDeactivation");
                    break;
                case ImpactType.ApplicationUpgrade:
                    writer.WriteStringValue("ApplicationUpgrade");
                    break;
                case ImpactType.FabricUpgrade:
                    writer.WriteStringValue("FabricUpgrade");
                    break;
                case ImpactType.Partition:
                    writer.WriteStringValue("Partition");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ImpactType");
            }
        }
    }
}
