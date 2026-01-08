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
    /// Converter for <see cref="LoadMetricInformation" />.
    /// </summary>
    internal class LoadMetricInformationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static LoadMetricInformation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static LoadMetricInformation GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var isBalancedBefore = default(bool?);
            var isBalancedAfter = default(bool?);
            var deviationBefore = default(double?);
            var deviationAfter = default(double?);
            var balancingThreshold = default(double?);
            var action = default(string);
            var activityThreshold = default(string);
            var clusterCapacity = default(string);
            var clusterLoad = default(string);
            var currentClusterLoad = default(double?);
            var clusterRemainingCapacity = default(string);
            var clusterCapacityRemaining = default(double?);
            var isClusterCapacityViolation = default(bool?);
            var nodeBufferPercentage = default(double?);
            var clusterBufferedCapacity = default(string);
            var bufferedClusterCapacityRemaining = default(double?);
            var clusterRemainingBufferedCapacity = default(string);
            var minNodeLoadValue = default(string);
            var minimumNodeLoad = default(double?);
            var minNodeLoadNodeId = default(NodeId);
            var maxNodeLoadValue = default(string);
            var maximumNodeLoad = default(double?);
            var maxNodeLoadNodeId = default(NodeId);
            var plannedLoadRemoval = default(double?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("IsBalancedBefore", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isBalancedBefore = reader.ReadValueAsBool();
                }
                else if (string.Compare("IsBalancedAfter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isBalancedAfter = reader.ReadValueAsBool();
                }
                else if (string.Compare("DeviationBefore", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deviationBefore = reader.ReadValueAsDouble();
                }
                else if (string.Compare("DeviationAfter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deviationAfter = reader.ReadValueAsDouble();
                }
                else if (string.Compare("BalancingThreshold", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    balancingThreshold = reader.ReadValueAsDouble();
                }
                else if (string.Compare("Action", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    action = reader.ReadValueAsString();
                }
                else if (string.Compare("ActivityThreshold", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    activityThreshold = reader.ReadValueAsString();
                }
                else if (string.Compare("ClusterCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterCapacity = reader.ReadValueAsString();
                }
                else if (string.Compare("ClusterLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterLoad = reader.ReadValueAsString();
                }
                else if (string.Compare("CurrentClusterLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentClusterLoad = reader.ReadValueAsDouble();
                }
                else if (string.Compare("ClusterRemainingCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterRemainingCapacity = reader.ReadValueAsString();
                }
                else if (string.Compare("ClusterCapacityRemaining", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterCapacityRemaining = reader.ReadValueAsDouble();
                }
                else if (string.Compare("IsClusterCapacityViolation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isClusterCapacityViolation = reader.ReadValueAsBool();
                }
                else if (string.Compare("NodeBufferPercentage", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeBufferPercentage = reader.ReadValueAsDouble();
                }
                else if (string.Compare("ClusterBufferedCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterBufferedCapacity = reader.ReadValueAsString();
                }
                else if (string.Compare("BufferedClusterCapacityRemaining", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    bufferedClusterCapacityRemaining = reader.ReadValueAsDouble();
                }
                else if (string.Compare("ClusterRemainingBufferedCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterRemainingBufferedCapacity = reader.ReadValueAsString();
                }
                else if (string.Compare("MinNodeLoadValue", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minNodeLoadValue = reader.ReadValueAsString();
                }
                else if (string.Compare("MinimumNodeLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minimumNodeLoad = reader.ReadValueAsDouble();
                }
                else if (string.Compare("MinNodeLoadNodeId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minNodeLoadNodeId = NodeIdConverter.Deserialize(reader);
                }
                else if (string.Compare("MaxNodeLoadValue", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxNodeLoadValue = reader.ReadValueAsString();
                }
                else if (string.Compare("MaximumNodeLoad", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maximumNodeLoad = reader.ReadValueAsDouble();
                }
                else if (string.Compare("MaxNodeLoadNodeId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxNodeLoadNodeId = NodeIdConverter.Deserialize(reader);
                }
                else if (string.Compare("PlannedLoadRemoval", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    plannedLoadRemoval = reader.ReadValueAsDouble();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new LoadMetricInformation(
                name: name,
                isBalancedBefore: isBalancedBefore,
                isBalancedAfter: isBalancedAfter,
                deviationBefore: deviationBefore,
                deviationAfter: deviationAfter,
                balancingThreshold: balancingThreshold,
                action: action,
                activityThreshold: activityThreshold,
                clusterCapacity: clusterCapacity,
                clusterLoad: clusterLoad,
                currentClusterLoad: currentClusterLoad,
                clusterRemainingCapacity: clusterRemainingCapacity,
                clusterCapacityRemaining: clusterCapacityRemaining,
                isClusterCapacityViolation: isClusterCapacityViolation,
                nodeBufferPercentage: nodeBufferPercentage,
                clusterBufferedCapacity: clusterBufferedCapacity,
                bufferedClusterCapacityRemaining: bufferedClusterCapacityRemaining,
                clusterRemainingBufferedCapacity: clusterRemainingBufferedCapacity,
                minNodeLoadValue: minNodeLoadValue,
                minimumNodeLoad: minimumNodeLoad,
                minNodeLoadNodeId: minNodeLoadNodeId,
                maxNodeLoadValue: maxNodeLoadValue,
                maximumNodeLoad: maximumNodeLoad,
                maxNodeLoadNodeId: maxNodeLoadNodeId,
                plannedLoadRemoval: plannedLoadRemoval);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, LoadMetricInformation obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.IsBalancedBefore != null)
            {
                writer.WriteProperty(obj.IsBalancedBefore, "IsBalancedBefore", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.IsBalancedAfter != null)
            {
                writer.WriteProperty(obj.IsBalancedAfter, "IsBalancedAfter", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.DeviationBefore != null)
            {
                writer.WriteProperty(obj.DeviationBefore, "DeviationBefore", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.DeviationAfter != null)
            {
                writer.WriteProperty(obj.DeviationAfter, "DeviationAfter", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.BalancingThreshold != null)
            {
                writer.WriteProperty(obj.BalancingThreshold, "BalancingThreshold", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.Action != null)
            {
                writer.WriteProperty(obj.Action, "Action", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ActivityThreshold != null)
            {
                writer.WriteProperty(obj.ActivityThreshold, "ActivityThreshold", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ClusterCapacity != null)
            {
                writer.WriteProperty(obj.ClusterCapacity, "ClusterCapacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ClusterLoad != null)
            {
                writer.WriteProperty(obj.ClusterLoad, "ClusterLoad", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CurrentClusterLoad != null)
            {
                writer.WriteProperty(obj.CurrentClusterLoad, "CurrentClusterLoad", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.ClusterRemainingCapacity != null)
            {
                writer.WriteProperty(obj.ClusterRemainingCapacity, "ClusterRemainingCapacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ClusterCapacityRemaining != null)
            {
                writer.WriteProperty(obj.ClusterCapacityRemaining, "ClusterCapacityRemaining", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.IsClusterCapacityViolation != null)
            {
                writer.WriteProperty(obj.IsClusterCapacityViolation, "IsClusterCapacityViolation", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.NodeBufferPercentage != null)
            {
                writer.WriteProperty(obj.NodeBufferPercentage, "NodeBufferPercentage", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.ClusterBufferedCapacity != null)
            {
                writer.WriteProperty(obj.ClusterBufferedCapacity, "ClusterBufferedCapacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.BufferedClusterCapacityRemaining != null)
            {
                writer.WriteProperty(obj.BufferedClusterCapacityRemaining, "BufferedClusterCapacityRemaining", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.ClusterRemainingBufferedCapacity != null)
            {
                writer.WriteProperty(obj.ClusterRemainingBufferedCapacity, "ClusterRemainingBufferedCapacity", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.MinNodeLoadValue != null)
            {
                writer.WriteProperty(obj.MinNodeLoadValue, "MinNodeLoadValue", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.MinimumNodeLoad != null)
            {
                writer.WriteProperty(obj.MinimumNodeLoad, "MinimumNodeLoad", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.MinNodeLoadNodeId != null)
            {
                writer.WriteProperty(obj.MinNodeLoadNodeId, "MinNodeLoadNodeId", NodeIdConverter.Serialize);
            }

            if (obj.MaxNodeLoadValue != null)
            {
                writer.WriteProperty(obj.MaxNodeLoadValue, "MaxNodeLoadValue", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.MaximumNodeLoad != null)
            {
                writer.WriteProperty(obj.MaximumNodeLoad, "MaximumNodeLoad", JsonWriterExtensions.WriteDoubleValue);
            }

            if (obj.MaxNodeLoadNodeId != null)
            {
                writer.WriteProperty(obj.MaxNodeLoadNodeId, "MaxNodeLoadNodeId", NodeIdConverter.Serialize);
            }

            if (obj.PlannedLoadRemoval != null)
            {
                writer.WriteProperty(obj.PlannedLoadRemoval, "PlannedLoadRemoval", JsonWriterExtensions.WriteDoubleValue);
            }

            writer.WriteEndObject();
        }
    }
}
