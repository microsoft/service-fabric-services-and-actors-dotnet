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
    /// Converter for <see cref="ServiceCorrelationScheme" />.
    /// </summary>
    internal class ServiceCorrelationSchemeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServiceCorrelationScheme? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServiceCorrelationScheme);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceCorrelationScheme.Invalid;
            }
            else if (string.Compare(value, "Affinity", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceCorrelationScheme.Affinity;
            }
            else if (string.Compare(value, "AlignedAffinity", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceCorrelationScheme.AlignedAffinity;
            }
            else if (string.Compare(value, "NonAlignedAffinity", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceCorrelationScheme.NonAlignedAffinity;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServiceCorrelationScheme? value)
        {
            switch (value)
            {
                case ServiceCorrelationScheme.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ServiceCorrelationScheme.Affinity:
                    writer.WriteStringValue("Affinity");
                    break;
                case ServiceCorrelationScheme.AlignedAffinity:
                    writer.WriteStringValue("AlignedAffinity");
                    break;
                case ServiceCorrelationScheme.NonAlignedAffinity:
                    writer.WriteStringValue("NonAlignedAffinity");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServiceCorrelationScheme");
            }
        }
    }
}
