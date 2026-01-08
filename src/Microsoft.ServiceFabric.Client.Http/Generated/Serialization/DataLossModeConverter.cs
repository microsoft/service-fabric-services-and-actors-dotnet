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
    /// Converter for <see cref="DataLossMode" />.
    /// </summary>
    internal class DataLossModeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static DataLossMode? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(DataLossMode);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DataLossMode.Invalid;
            }
            else if (string.Compare(value, "PartialDataLoss", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DataLossMode.PartialDataLoss;
            }
            else if (string.Compare(value, "FullDataLoss", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DataLossMode.FullDataLoss;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, DataLossMode? value)
        {
            switch (value)
            {
                case DataLossMode.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case DataLossMode.PartialDataLoss:
                    writer.WriteStringValue("PartialDataLoss");
                    break;
                case DataLossMode.FullDataLoss:
                    writer.WriteStringValue("FullDataLoss");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type DataLossMode");
            }
        }
    }
}
