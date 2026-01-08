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
    /// Converter for <see cref="StatefulServiceUpdateDescription" />.
    /// </summary>
    internal class StatefulServiceUpdateDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static StatefulServiceUpdateDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static StatefulServiceUpdateDescription GetFromJsonProperties(JsonReader reader)
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
            var targetReplicaSetSize = default(int?);
            var minReplicaSetSize = default(int?);
            var replicaRestartWaitDurationSeconds = default(string);
            var quorumLossWaitDurationSeconds = default(string);
            var standByReplicaKeepDurationSeconds = default(string);
            var servicePlacementTimeLimitSeconds = default(string);
            var dropSourceReplicaOnMove = default(bool?);
            var replicaLifecycleDescription = default(ReplicaLifecycleDescription);
            var auxiliaryReplicaCount = default(int?);
            var serviceSensitivityDescription = default(ServiceSensitivityDescription);

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
                else if (string.Compare("TargetReplicaSetSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    targetReplicaSetSize = reader.ReadValueAsInt();
                }
                else if (string.Compare("MinReplicaSetSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minReplicaSetSize = reader.ReadValueAsInt();
                }
                else if (string.Compare("ReplicaRestartWaitDurationSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaRestartWaitDurationSeconds = reader.ReadValueAsString();
                }
                else if (string.Compare("QuorumLossWaitDurationSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    quorumLossWaitDurationSeconds = reader.ReadValueAsString();
                }
                else if (string.Compare("StandByReplicaKeepDurationSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    standByReplicaKeepDurationSeconds = reader.ReadValueAsString();
                }
                else if (string.Compare("ServicePlacementTimeLimitSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePlacementTimeLimitSeconds = reader.ReadValueAsString();
                }
                else if (string.Compare("DropSourceReplicaOnMove", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    dropSourceReplicaOnMove = reader.ReadValueAsBool();
                }
                else if (string.Compare("ReplicaLifecycleDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaLifecycleDescription = ReplicaLifecycleDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("AuxiliaryReplicaCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    auxiliaryReplicaCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("ServiceSensitivityDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceSensitivityDescription = ServiceSensitivityDescriptionConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new StatefulServiceUpdateDescription(
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
                targetReplicaSetSize: targetReplicaSetSize,
                minReplicaSetSize: minReplicaSetSize,
                replicaRestartWaitDurationSeconds: replicaRestartWaitDurationSeconds,
                quorumLossWaitDurationSeconds: quorumLossWaitDurationSeconds,
                standByReplicaKeepDurationSeconds: standByReplicaKeepDurationSeconds,
                servicePlacementTimeLimitSeconds: servicePlacementTimeLimitSeconds,
                dropSourceReplicaOnMove: dropSourceReplicaOnMove,
                replicaLifecycleDescription: replicaLifecycleDescription,
                auxiliaryReplicaCount: auxiliaryReplicaCount,
                serviceSensitivityDescription: serviceSensitivityDescription);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, StatefulServiceUpdateDescription obj)
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

            if (obj.TargetReplicaSetSize != null)
            {
                writer.WriteProperty(obj.TargetReplicaSetSize, "TargetReplicaSetSize", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MinReplicaSetSize != null)
            {
                writer.WriteProperty(obj.MinReplicaSetSize, "MinReplicaSetSize", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.ReplicaRestartWaitDurationSeconds != null)
            {
                writer.WriteProperty(obj.ReplicaRestartWaitDurationSeconds, "ReplicaRestartWaitDurationSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.QuorumLossWaitDurationSeconds != null)
            {
                writer.WriteProperty(obj.QuorumLossWaitDurationSeconds, "QuorumLossWaitDurationSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.StandByReplicaKeepDurationSeconds != null)
            {
                writer.WriteProperty(obj.StandByReplicaKeepDurationSeconds, "StandByReplicaKeepDurationSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServicePlacementTimeLimitSeconds != null)
            {
                writer.WriteProperty(obj.ServicePlacementTimeLimitSeconds, "ServicePlacementTimeLimitSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.DropSourceReplicaOnMove != null)
            {
                writer.WriteProperty(obj.DropSourceReplicaOnMove, "DropSourceReplicaOnMove", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.ReplicaLifecycleDescription != null)
            {
                writer.WriteProperty(obj.ReplicaLifecycleDescription, "ReplicaLifecycleDescription", ReplicaLifecycleDescriptionConverter.Serialize);
            }

            if (obj.AuxiliaryReplicaCount != null)
            {
                writer.WriteProperty(obj.AuxiliaryReplicaCount, "AuxiliaryReplicaCount", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.ServiceSensitivityDescription != null)
            {
                writer.WriteProperty(obj.ServiceSensitivityDescription, "ServiceSensitivityDescription", ServiceSensitivityDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
