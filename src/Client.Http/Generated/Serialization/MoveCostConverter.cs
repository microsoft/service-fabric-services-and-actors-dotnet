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
    /// Converter for <see cref="MoveCost" />.
    /// </summary>
    internal class MoveCostConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static MoveCost? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(MoveCost);

            if (string.Compare(value, "Zero", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MoveCost.Zero;
            }
            else if (string.Compare(value, "Low", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MoveCost.Low;
            }
            else if (string.Compare(value, "Medium", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MoveCost.Medium;
            }
            else if (string.Compare(value, "High", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MoveCost.High;
            }
            else if (string.Compare(value, "VeryHigh", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MoveCost.VeryHigh;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, MoveCost? value)
        {
            switch (value)
            {
                case MoveCost.Zero:
                    writer.WriteStringValue("Zero");
                    break;
                case MoveCost.Low:
                    writer.WriteStringValue("Low");
                    break;
                case MoveCost.Medium:
                    writer.WriteStringValue("Medium");
                    break;
                case MoveCost.High:
                    writer.WriteStringValue("High");
                    break;
                case MoveCost.VeryHigh:
                    writer.WriteStringValue("VeryHigh");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type MoveCost");
            }
        }
    }
}
