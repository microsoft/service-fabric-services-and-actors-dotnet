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
    /// Converter for <see cref="SizeTypes" />.
    /// </summary>
    internal class SizeTypesConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static SizeTypes? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(SizeTypes);

            if (string.Compare(value, "Small", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SizeTypes.Small;
            }
            else if (string.Compare(value, "Medium", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SizeTypes.Medium;
            }
            else if (string.Compare(value, "Large", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = SizeTypes.Large;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, SizeTypes? value)
        {
            switch (value)
            {
                case SizeTypes.Small:
                    writer.WriteStringValue("Small");
                    break;
                case SizeTypes.Medium:
                    writer.WriteStringValue("Medium");
                    break;
                case SizeTypes.Large:
                    writer.WriteStringValue("Large");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type SizeTypes");
            }
        }
    }
}
