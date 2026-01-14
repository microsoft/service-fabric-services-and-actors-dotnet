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
    /// Converter for <see cref="UpgradeMode" />.
    /// </summary>
    internal class UpgradeModeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static UpgradeMode? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(UpgradeMode);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeMode.Invalid;
            }
            else if (string.Compare(value, "UnmonitoredAuto", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeMode.UnmonitoredAuto;
            }
            else if (string.Compare(value, "UnmonitoredManual", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeMode.UnmonitoredManual;
            }
            else if (string.Compare(value, "Monitored", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeMode.Monitored;
            }
            else if (string.Compare(value, "UnmonitoredDeferred", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeMode.UnmonitoredDeferred;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, UpgradeMode? value)
        {
            switch (value)
            {
                case UpgradeMode.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case UpgradeMode.UnmonitoredAuto:
                    writer.WriteStringValue("UnmonitoredAuto");
                    break;
                case UpgradeMode.UnmonitoredManual:
                    writer.WriteStringValue("UnmonitoredManual");
                    break;
                case UpgradeMode.Monitored:
                    writer.WriteStringValue("Monitored");
                    break;
                case UpgradeMode.UnmonitoredDeferred:
                    writer.WriteStringValue("UnmonitoredDeferred");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type UpgradeMode");
            }
        }
    }
}
