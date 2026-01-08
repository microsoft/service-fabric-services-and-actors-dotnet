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
    /// Converter for <see cref="ServiceLoadMetricWeight" />.
    /// </summary>
    internal class ServiceLoadMetricWeightConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServiceLoadMetricWeight? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServiceLoadMetricWeight);

            if (string.Compare(value, "Zero", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceLoadMetricWeight.Zero;
            }
            else if (string.Compare(value, "Low", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceLoadMetricWeight.Low;
            }
            else if (string.Compare(value, "Medium", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceLoadMetricWeight.Medium;
            }
            else if (string.Compare(value, "High", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceLoadMetricWeight.High;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServiceLoadMetricWeight? value)
        {
            switch (value)
            {
                case ServiceLoadMetricWeight.Zero:
                    writer.WriteStringValue("Zero");
                    break;
                case ServiceLoadMetricWeight.Low:
                    writer.WriteStringValue("Low");
                    break;
                case ServiceLoadMetricWeight.Medium:
                    writer.WriteStringValue("Medium");
                    break;
                case ServiceLoadMetricWeight.High:
                    writer.WriteStringValue("High");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServiceLoadMetricWeight");
            }
        }
    }
}
