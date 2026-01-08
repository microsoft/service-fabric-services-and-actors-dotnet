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
    /// Converter for <see cref="RollingUpgradeMode" />.
    /// </summary>
    internal class RollingUpgradeModeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static RollingUpgradeMode? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(RollingUpgradeMode);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RollingUpgradeMode.Invalid;
            }
            else if (string.Compare(value, "UnmonitoredAuto", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RollingUpgradeMode.UnmonitoredAuto;
            }
            else if (string.Compare(value, "UnmonitoredManual", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RollingUpgradeMode.UnmonitoredManual;
            }
            else if (string.Compare(value, "Monitored", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RollingUpgradeMode.Monitored;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, RollingUpgradeMode? value)
        {
            switch (value)
            {
                case RollingUpgradeMode.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case RollingUpgradeMode.UnmonitoredAuto:
                    writer.WriteStringValue("UnmonitoredAuto");
                    break;
                case RollingUpgradeMode.UnmonitoredManual:
                    writer.WriteStringValue("UnmonitoredManual");
                    break;
                case RollingUpgradeMode.Monitored:
                    writer.WriteStringValue("Monitored");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type RollingUpgradeMode");
            }
        }
    }
}
