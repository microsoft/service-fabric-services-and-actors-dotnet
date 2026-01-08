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
    /// Converter for <see cref="ServicePartitionStatus" />.
    /// </summary>
    internal class ServicePartitionStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServicePartitionStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServicePartitionStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionStatus.Invalid;
            }
            else if (string.Compare(value, "Ready", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionStatus.Ready;
            }
            else if (string.Compare(value, "NotReady", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionStatus.NotReady;
            }
            else if (string.Compare(value, "InQuorumLoss", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionStatus.InQuorumLoss;
            }
            else if (string.Compare(value, "Reconfiguring", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionStatus.Reconfiguring;
            }
            else if (string.Compare(value, "Deleting", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServicePartitionStatus.Deleting;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServicePartitionStatus? value)
        {
            switch (value)
            {
                case ServicePartitionStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ServicePartitionStatus.Ready:
                    writer.WriteStringValue("Ready");
                    break;
                case ServicePartitionStatus.NotReady:
                    writer.WriteStringValue("NotReady");
                    break;
                case ServicePartitionStatus.InQuorumLoss:
                    writer.WriteStringValue("InQuorumLoss");
                    break;
                case ServicePartitionStatus.Reconfiguring:
                    writer.WriteStringValue("Reconfiguring");
                    break;
                case ServicePartitionStatus.Deleting:
                    writer.WriteStringValue("Deleting");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServicePartitionStatus");
            }
        }
    }
}
