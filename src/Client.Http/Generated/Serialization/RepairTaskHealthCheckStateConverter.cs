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
    /// Converter for <see cref="RepairTaskHealthCheckState" />.
    /// </summary>
    internal class RepairTaskHealthCheckStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static RepairTaskHealthCheckState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(RepairTaskHealthCheckState);

            if (string.Compare(value, "NotStarted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RepairTaskHealthCheckState.NotStarted;
            }
            else if (string.Compare(value, "InProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RepairTaskHealthCheckState.InProgress;
            }
            else if (string.Compare(value, "Succeeded", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RepairTaskHealthCheckState.Succeeded;
            }
            else if (string.Compare(value, "Skipped", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RepairTaskHealthCheckState.Skipped;
            }
            else if (string.Compare(value, "TimedOut", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RepairTaskHealthCheckState.TimedOut;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, RepairTaskHealthCheckState? value)
        {
            switch (value)
            {
                case RepairTaskHealthCheckState.NotStarted:
                    writer.WriteStringValue("NotStarted");
                    break;
                case RepairTaskHealthCheckState.InProgress:
                    writer.WriteStringValue("InProgress");
                    break;
                case RepairTaskHealthCheckState.Succeeded:
                    writer.WriteStringValue("Succeeded");
                    break;
                case RepairTaskHealthCheckState.Skipped:
                    writer.WriteStringValue("Skipped");
                    break;
                case RepairTaskHealthCheckState.TimedOut:
                    writer.WriteStringValue("TimedOut");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type RepairTaskHealthCheckState");
            }
        }
    }
}
