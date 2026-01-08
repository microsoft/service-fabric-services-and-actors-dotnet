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
    /// Converter for <see cref="OperationType" />.
    /// </summary>
    internal class OperationTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static OperationType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(OperationType);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationType.Invalid;
            }
            else if (string.Compare(value, "PartitionDataLoss", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationType.PartitionDataLoss;
            }
            else if (string.Compare(value, "PartitionQuorumLoss", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationType.PartitionQuorumLoss;
            }
            else if (string.Compare(value, "PartitionRestart", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationType.PartitionRestart;
            }
            else if (string.Compare(value, "NodeTransition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationType.NodeTransition;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, OperationType? value)
        {
            switch (value)
            {
                case OperationType.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case OperationType.PartitionDataLoss:
                    writer.WriteStringValue("PartitionDataLoss");
                    break;
                case OperationType.PartitionQuorumLoss:
                    writer.WriteStringValue("PartitionQuorumLoss");
                    break;
                case OperationType.PartitionRestart:
                    writer.WriteStringValue("PartitionRestart");
                    break;
                case OperationType.NodeTransition:
                    writer.WriteStringValue("NodeTransition");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type OperationType");
            }
        }
    }
}
