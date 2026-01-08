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
    /// Converter for <see cref="MonitoredUpgradeHealthCheckPhase" />.
    /// </summary>
    internal class MonitoredUpgradeHealthCheckPhaseConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static MonitoredUpgradeHealthCheckPhase? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(MonitoredUpgradeHealthCheckPhase);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MonitoredUpgradeHealthCheckPhase.Invalid;
            }
            else if (string.Compare(value, "WaitDuration", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MonitoredUpgradeHealthCheckPhase.WaitDuration;
            }
            else if (string.Compare(value, "StableDuration", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MonitoredUpgradeHealthCheckPhase.StableDuration;
            }
            else if (string.Compare(value, "Retry", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = MonitoredUpgradeHealthCheckPhase.Retry;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, MonitoredUpgradeHealthCheckPhase? value)
        {
            switch (value)
            {
                case MonitoredUpgradeHealthCheckPhase.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case MonitoredUpgradeHealthCheckPhase.WaitDuration:
                    writer.WriteStringValue("WaitDuration");
                    break;
                case MonitoredUpgradeHealthCheckPhase.StableDuration:
                    writer.WriteStringValue("StableDuration");
                    break;
                case MonitoredUpgradeHealthCheckPhase.Retry:
                    writer.WriteStringValue("Retry");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type MonitoredUpgradeHealthCheckPhase");
            }
        }
    }
}
