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
    /// Converter for <see cref="NodeStatusFilter" />.
    /// </summary>
    internal class NodeStatusFilterConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static NodeStatusFilter? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(NodeStatusFilter);

            if (string.Compare(value, "default", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.Default;
            }
            else if (string.Compare(value, "all", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.All;
            }
            else if (string.Compare(value, "up", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.Up;
            }
            else if (string.Compare(value, "down", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.Down;
            }
            else if (string.Compare(value, "enabling", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.Enabling;
            }
            else if (string.Compare(value, "disabling", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.Disabling;
            }
            else if (string.Compare(value, "disabled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.Disabled;
            }
            else if (string.Compare(value, "unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.Unknown;
            }
            else if (string.Compare(value, "removed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeStatusFilter.Removed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, NodeStatusFilter? value)
        {
            switch (value)
            {
                case NodeStatusFilter.Default:
                    writer.WriteStringValue("default");
                    break;
                case NodeStatusFilter.All:
                    writer.WriteStringValue("all");
                    break;
                case NodeStatusFilter.Up:
                    writer.WriteStringValue("up");
                    break;
                case NodeStatusFilter.Down:
                    writer.WriteStringValue("down");
                    break;
                case NodeStatusFilter.Enabling:
                    writer.WriteStringValue("enabling");
                    break;
                case NodeStatusFilter.Disabling:
                    writer.WriteStringValue("disabling");
                    break;
                case NodeStatusFilter.Disabled:
                    writer.WriteStringValue("disabled");
                    break;
                case NodeStatusFilter.Unknown:
                    writer.WriteStringValue("unknown");
                    break;
                case NodeStatusFilter.Removed:
                    writer.WriteStringValue("removed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type NodeStatusFilter");
            }
        }
    }
}
