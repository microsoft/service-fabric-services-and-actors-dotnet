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
    /// Converter for <see cref="ReconfigurationType" />.
    /// </summary>
    internal class ReconfigurationTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ReconfigurationType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ReconfigurationType);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationType.Unknown;
            }
            else if (string.Compare(value, "SwapPrimary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationType.SwapPrimary;
            }
            else if (string.Compare(value, "Failover", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationType.Failover;
            }
            else if (string.Compare(value, "Other", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReconfigurationType.Other;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ReconfigurationType? value)
        {
            switch (value)
            {
                case ReconfigurationType.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case ReconfigurationType.SwapPrimary:
                    writer.WriteStringValue("SwapPrimary");
                    break;
                case ReconfigurationType.Failover:
                    writer.WriteStringValue("Failover");
                    break;
                case ReconfigurationType.Other:
                    writer.WriteStringValue("Other");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ReconfigurationType");
            }
        }
    }
}
