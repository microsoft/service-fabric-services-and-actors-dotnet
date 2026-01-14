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
    /// Converter for <see cref="ChaosStatus" />.
    /// </summary>
    internal class ChaosStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ChaosStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ChaosStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosStatus.Invalid;
            }
            else if (string.Compare(value, "Running", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosStatus.Running;
            }
            else if (string.Compare(value, "Stopped", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosStatus.Stopped;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ChaosStatus? value)
        {
            switch (value)
            {
                case ChaosStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ChaosStatus.Running:
                    writer.WriteStringValue("Running");
                    break;
                case ChaosStatus.Stopped:
                    writer.WriteStringValue("Stopped");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ChaosStatus");
            }
        }
    }
}
