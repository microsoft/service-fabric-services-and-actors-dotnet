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
    /// Converter for <see cref="StatelessServiceUpdateDescription" />.
    /// </summary>
    internal class StatelessServiceUpdateDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceUpdateDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceUpdateDescription GetFromJsonProperties(JsonReader reader)
        {
            var flags = default(string);
            var placementConstraints = default(string);
            var correlationScheme = default(IEnumerable<ServiceCorrelationDescription>);
            var loadMetrics = default(IEnumerable<ServiceLoadMetricDescription>);
            var servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>);
            var defaultMoveCost = default(MoveCost?);
            var scalingPolicies = default(IEnumerable<ScalingPolicyDescription>);
            var serviceDnsName = default(string);
            var tagsForPlacement = default(NodeTagsDescription);
            var tagsForRunning = default(NodeTagsDescription);
            var instanceCount = default(int?);
            var minInstanceCount = default(int?);
            var minInstancePercentage = default(int?);
            var instanceCloseDelayDurationSeconds = default(string);
            var instanceLifecycleDescription = default(InstanceLifecycleDescription);
            var instanceRestartWaitDurationSeconds = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Flags", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    flags = reader.ReadValueAsString();
                }
                else if (string.Compare("PlacementConstraints", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    placementConstraints = reader.ReadValueAsString();
                }
                else if (string.Compare("CorrelationScheme", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    correlationScheme = reader.ReadList(ServiceCorrelationDescriptionConverter.Deserialize);
                }
                else if (string.Compare("LoadMetrics", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    loadMetrics = reader.ReadList(ServiceLoadMetricDescriptionConverter.Deserialize);
                }
                else if (string.Compare("ServicePlacementPolicies", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePlacementPolicies = reader.ReadList(ServicePlacementPolicyDescriptionConverter.Deserialize);
                }
                else if (string.Compare("DefaultMoveCost", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    defaultMoveCost = MoveCostConverter.Deserialize(reader);
                }
                else if (string.Compare("ScalingPolicies", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scalingPolicies = reader.ReadList(ScalingPolicyDescriptionConverter.Deserialize);
                }
                else if (string.Compare("ServiceDnsName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceDnsName = reader.ReadValueAsString();
                }
                else if (string.Compare("TagsForPlacement", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tagsForPlacement = NodeTagsDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("TagsForRunning", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tagsForRunning = NodeTagsDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("InstanceCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("MinInstanceCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minInstanceCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("MinInstancePercentage", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minInstancePercentage = reader.ReadValueAsInt();
                }
                else if (string.Compare("InstanceCloseDelayDurationSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceCloseDelayDurationSeconds = reader.ReadValueAsString();
                }
                else if (string.Compare("InstanceLifecycleDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceLifecycleDescription = InstanceLifecycleDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("InstanceRestartWaitDurationSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceRestartWaitDurationSeconds = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new StatelessServiceUpdateDescription(
                flags: flags,
                placementConstraints: placementConstraints,
                correlationScheme: correlationScheme,
                loadMetrics: loadMetrics,
                servicePlacementPolicies: servicePlacementPolicies,
                defaultMoveCost: defaultMoveCost,
                scalingPolicies: scalingPolicies,
                serviceDnsName: serviceDnsName,
                tagsForPlacement: tagsForPlacement,
                tagsForRunning: tagsForRunning,
                instanceCount: instanceCount,
                minInstanceCount: minInstanceCount,
                minInstancePercentage: minInstancePercentage,
                instanceCloseDelayDurationSeconds: instanceCloseDelayDurationSeconds,
                instanceLifecycleDescription: instanceLifecycleDescription,
                instanceRestartWaitDurationSeconds: instanceRestartWaitDurationSeconds);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, StatelessServiceUpdateDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceKind, "ServiceKind", ServiceKindConverter.Serialize);
            writer.WriteProperty(obj.DefaultMoveCost, "DefaultMoveCost", MoveCostConverter.Serialize);
            if (obj.Flags != null)
            {
                writer.WriteProperty(obj.Flags, "Flags", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PlacementConstraints != null)
            {
                writer.WriteProperty(obj.PlacementConstraints, "PlacementConstraints", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CorrelationScheme != null)
            {
                writer.WriteEnumerableProperty(obj.CorrelationScheme, "CorrelationScheme", ServiceCorrelationDescriptionConverter.Serialize);
            }

            if (obj.LoadMetrics != null)
            {
                writer.WriteEnumerableProperty(obj.LoadMetrics, "LoadMetrics", ServiceLoadMetricDescriptionConverter.Serialize);
            }

            if (obj.ServicePlacementPolicies != null)
            {
                writer.WriteEnumerableProperty(obj.ServicePlacementPolicies, "ServicePlacementPolicies", ServicePlacementPolicyDescriptionConverter.Serialize);
            }

            if (obj.ScalingPolicies != null)
            {
                writer.WriteEnumerableProperty(obj.ScalingPolicies, "ScalingPolicies", ScalingPolicyDescriptionConverter.Serialize);
            }

            if (obj.ServiceDnsName != null)
            {
                writer.WriteProperty(obj.ServiceDnsName, "ServiceDnsName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.TagsForPlacement != null)
            {
                writer.WriteProperty(obj.TagsForPlacement, "TagsForPlacement", NodeTagsDescriptionConverter.Serialize);
            }

            if (obj.TagsForRunning != null)
            {
                writer.WriteProperty(obj.TagsForRunning, "TagsForRunning", NodeTagsDescriptionConverter.Serialize);
            }

            if (obj.InstanceCount != null)
            {
                writer.WriteProperty(obj.InstanceCount, "InstanceCount", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MinInstanceCount != null)
            {
                writer.WriteProperty(obj.MinInstanceCount, "MinInstanceCount", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MinInstancePercentage != null)
            {
                writer.WriteProperty(obj.MinInstancePercentage, "MinInstancePercentage", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.InstanceCloseDelayDurationSeconds != null)
            {
                writer.WriteProperty(obj.InstanceCloseDelayDurationSeconds, "InstanceCloseDelayDurationSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.InstanceLifecycleDescription != null)
            {
                writer.WriteProperty(obj.InstanceLifecycleDescription, "InstanceLifecycleDescription", InstanceLifecycleDescriptionConverter.Serialize);
            }

            if (obj.InstanceRestartWaitDurationSeconds != null)
            {
                writer.WriteProperty(obj.InstanceRestartWaitDurationSeconds, "InstanceRestartWaitDurationSeconds", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
