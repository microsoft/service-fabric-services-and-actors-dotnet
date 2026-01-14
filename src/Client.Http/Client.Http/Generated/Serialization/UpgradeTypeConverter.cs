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
    /// Converter for <see cref="UpgradeType" />.
    /// </summary>
    internal class UpgradeTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static UpgradeType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(UpgradeType);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeType.Invalid;
            }
            else if (string.Compare(value, "Rolling", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeType.Rolling;
            }
            else if (string.Compare(value, "Rolling_ForceRestart", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeType.Rolling_ForceRestart;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, UpgradeType? value)
        {
            switch (value)
            {
                case UpgradeType.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case UpgradeType.Rolling:
                    writer.WriteStringValue("Rolling");
                    break;
                case UpgradeType.Rolling_ForceRestart:
                    writer.WriteStringValue("Rolling_ForceRestart");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type UpgradeType");
            }
        }
    }
}
