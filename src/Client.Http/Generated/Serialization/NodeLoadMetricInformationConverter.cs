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
    /// Converter for <see cref="NodeLoadMetricInformation" />.
    /// </summary>
    internal class NodeLoadMetricInformationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeLoadMetricInformation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NodeLoadMetricInformation GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var nodeCapacity = default(string);
            var nodeLoad = default(string);
            var nodeRemainingCapacity = default(string);
            var isCapacityViolation = default(bool?);
            var nodeBufferedCapacity = default(string);
            var nodeRemainingBufferedCapacity = default(string);
            var currentNodeLoad = default(double?);
            var nodeCapacityRemaining = default(double?);
            var bufferedNodeCapacityRemaining = default(double?);
            var plannedNodeLoadRemoval = default(double?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeCapacity = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeLoad = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeRemainingCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeRemainingCapacity = reader.ReadValueAsString();
                }
                else if (string.Compare("IsCapacityViolation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isCapacityViolation = reader.ReadValueAsBool();
                }
                else if (string.Compare("NodeBufferedCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeBufferedCapacity = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeRemainingBufferedCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeRemainingBufferedCapacity = reader.ReadValueAsString();
                }
                else if (string.Compare("CurrentNodeLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentNodeLoad = reader.ReadValueAsDouble();
                }
                else if (string.Compare("NodeCapacityRemaining", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeCapacityRemaining = reader.ReadValueAsDouble();
                }
                else if (string.Compare("BufferedNodeCapacityRemaining", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    bufferedNodeCapacityRemaining = reader.ReadValueAsDouble();
                }
                else if (string.Compare("PlannedNodeLoadRemoval", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    plannedNodeLoadRemoval = reader.ReadValueAsDouble();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeLoadMetricInformation(
                name: name,
                nodeCapacity: nodeCapacity,
                nodeLoad: nodeLoad,
                nodeRemainingCapacity: nodeRemainingCapacity,
                isCapacityViolation: isCapacityViolation,
                nodeBufferedCapacity: nodeBufferedCapacity,
                nodeRemainingBufferedCapacity: nodeRemainingBufferedCapacity,
                currentNodeLoad: currentNodeLoad,
                nodeCapacityRemaining: nodeCapacityRemaining,
                bufferedNodeCapacityRemaining: bufferedNodeCapacityRemaining,
                plannedNodeLoadRemoval: plannedNodeLoadRemoval);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeLoadMetricInformation obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NodeCapacity != null)
            {
                writer.WriteProperty(obj.NodeCapacity, "NodeCapacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NodeLoad != null)
            {
                writer.WriteProperty(obj.NodeLoad, "NodeLoad", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NodeRemainingCapacity != null)
            {
                writer.WriteProperty(obj.NodeRemainingCapacity, "NodeRemainingCapacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.IsCapacityViolation != null)
            {
                writer.WriteProperty(obj.IsCapacityViolation, "IsCapacityViolation", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.NodeBufferedCapacity != null)
            {
                writer.WriteProperty(obj.NodeBufferedCapacity, "NodeBufferedCapacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NodeRemainingBufferedCapacity != null)
            {
                writer.WriteProperty(obj.NodeRemainingBufferedCapacity, "NodeRemainingBufferedCapacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CurrentNodeLoad != null)
            {
                writer.WriteProperty(obj.CurrentNodeLoad, "CurrentNodeLoad", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.NodeCapacityRemaining != null)
            {
                writer.WriteProperty(obj.NodeCapacityRemaining, "NodeCapacityRemaining", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.BufferedNodeCapacityRemaining != null)
            {
                writer.WriteProperty(obj.BufferedNodeCapacityRemaining, "BufferedNodeCapacityRemaining", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.PlannedNodeLoadRemoval != null)
            {
                writer.WriteProperty(obj.PlannedNodeLoadRemoval, "PlannedNodeLoadRemoval", JsonWriterExtensions.WriteDoubleValue);
            }

            writer.WriteEndObject();
        }
    }
}
