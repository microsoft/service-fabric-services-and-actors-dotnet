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
    /// Converter for <see cref="ServicePartitionKind" />.
    /// </summary>
    internal class ServicePartitionKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServicePartitionKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServicePartitionKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionKind.Invalid;
            }
            else if (string.Compare(value, "Singleton", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionKind.Singleton;
            }
            else if (string.Compare(value, "Int64Range", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionKind.Int64Range;
            }
            else if (string.Compare(value, "Named", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionKind.Named;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServicePartitionKind? value)
        {
            switch (value)
            {
                case ServicePartitionKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ServicePartitionKind.Singleton:
                    writer.WriteStringValue("Singleton");
                    break;
                case ServicePartitionKind.Int64Range:
                    writer.WriteStringValue("Int64Range");
                    break;
                case ServicePartitionKind.Named:
                    writer.WriteStringValue("Named");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServicePartitionKind");
            }
        }
    }
}
