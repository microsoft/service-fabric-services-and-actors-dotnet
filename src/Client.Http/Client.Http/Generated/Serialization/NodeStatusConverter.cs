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
    /// Converter for <see cref="NodeStatus" />.
    /// </summary>
    internal class NodeStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static NodeStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(NodeStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatus.Invalid;
            }
            else if (string.Compare(value, "Up", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatus.Up;
            }
            else if (string.Compare(value, "Down", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatus.Down;
            }
            else if (string.Compare(value, "Enabling", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatus.Enabling;
            }
            else if (string.Compare(value, "Disabling", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatus.Disabling;
            }
            else if (string.Compare(value, "Disabled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatus.Disabled;
            }
            else if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatus.Unknown;
            }
            else if (string.Compare(value, "Removed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatus.Removed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, NodeStatus? value)
        {
            switch (value)
            {
                case NodeStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case NodeStatus.Up:
                    writer.WriteStringValue("Up");
                    break;
                case NodeStatus.Down:
                    writer.WriteStringValue("Down");
                    break;
                case NodeStatus.Enabling:
                    writer.WriteStringValue("Enabling");
                    break;
                case NodeStatus.Disabling:
                    writer.WriteStringValue("Disabling");
                    break;
                case NodeStatus.Disabled:
                    writer.WriteStringValue("Disabled");
                    break;
                case NodeStatus.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case NodeStatus.Removed:
                    writer.WriteStringValue("Removed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type NodeStatus");
            }
        }
    }
}
