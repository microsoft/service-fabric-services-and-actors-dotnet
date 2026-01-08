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
    /// Converter for <see cref="PropertyValueKind" />.
    /// </summary>
    internal class PropertyValueKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static PropertyValueKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(PropertyValueKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyValueKind.Invalid;
            }
            else if (string.Compare(value, "Binary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyValueKind.Binary;
            }
            else if (string.Compare(value, "Int64", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyValueKind.Int64;
            }
            else if (string.Compare(value, "Double", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyValueKind.Double;
            }
            else if (string.Compare(value, "String", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyValueKind.String;
            }
            else if (string.Compare(value, "Guid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyValueKind.Guid;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, PropertyValueKind? value)
        {
            switch (value)
            {
                case PropertyValueKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case PropertyValueKind.Binary:
                    writer.WriteStringValue("Binary");
                    break;
                case PropertyValueKind.Int64:
                    writer.WriteStringValue("Int64");
                    break;
                case PropertyValueKind.Double:
                    writer.WriteStringValue("Double");
                    break;
                case PropertyValueKind.String:
                    writer.WriteStringValue("String");
                    break;
                case PropertyValueKind.Guid:
                    writer.WriteStringValue("Guid");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type PropertyValueKind");
            }
        }
    }
}
