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
    /// Converter for <see cref="UpgradeSortOrder" />.
    /// </summary>
    internal class UpgradeSortOrderConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static UpgradeSortOrder? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(UpgradeSortOrder);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeSortOrder.Invalid;
            }
            else if (string.Compare(value, "Default", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeSortOrder.Default;
            }
            else if (string.Compare(value, "Numeric", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeSortOrder.Numeric;
            }
            else if (string.Compare(value, "Lexicographical", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeSortOrder.Lexicographical;
            }
            else if (string.Compare(value, "ReverseNumeric", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeSortOrder.ReverseNumeric;
            }
            else if (string.Compare(value, "ReverseLexicographical", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeSortOrder.ReverseLexicographical;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, UpgradeSortOrder? value)
        {
            switch (value)
            {
                case UpgradeSortOrder.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case UpgradeSortOrder.Default:
                    writer.WriteStringValue("Default");
                    break;
                case UpgradeSortOrder.Numeric:
                    writer.WriteStringValue("Numeric");
                    break;
                case UpgradeSortOrder.Lexicographical:
                    writer.WriteStringValue("Lexicographical");
                    break;
                case UpgradeSortOrder.ReverseNumeric:
                    writer.WriteStringValue("ReverseNumeric");
                    break;
                case UpgradeSortOrder.ReverseLexicographical:
                    writer.WriteStringValue("ReverseLexicographical");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type UpgradeSortOrder");
            }
        }
    }
}
