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
    /// Converter for <see cref="HealthState" />.
    /// </summary>
    internal class HealthStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static HealthState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(HealthState);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthState.Invalid;
            }
            else if (string.Compare(value, "Ok", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthState.Ok;
            }
            else if (string.Compare(value, "Warning", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthState.Warning;
            }
            else if (string.Compare(value, "Error", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthState.Error;
            }
            else if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HealthState.Unknown;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, HealthState? value)
        {
            switch (value)
            {
                case HealthState.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case HealthState.Ok:
                    writer.WriteStringValue("Ok");
                    break;
                case HealthState.Warning:
                    writer.WriteStringValue("Warning");
                    break;
                case HealthState.Error:
                    writer.WriteStringValue("Error");
                    break;
                case HealthState.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type HealthState");
            }
        }
    }
}
