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
    /// Converter for <see cref="StatelessServiceDescription" />.
    /// </summary>
    internal class StatelessServiceDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceDescription GetFromJsonProperties(JsonReader reader)
        {
            var applicationName = default(ApplicationName);
            var serviceName = default(ServiceName);
            var serviceTypeName = default(string);
            var initializationData = default(byte[]);
            var partitionDescription = default(PartitionSchemeDescription);
            var placementConstraints = default(string);
            var correlationScheme = default(IEnumerable<ServiceCorrelationDescription>);
            var serviceLoadMetrics = default(IEnumerable<ServiceLoadMetricDescription>);
            var servicePlacementPolicies = default(IEnumerable<ServicePlacementPolicyDescription>);
            var defaultMoveCost = default(MoveCost?);
            var isDefaultMoveCostSpecified = default(bool?);
            var servicePackageActivationMode = default(ServicePackageActivationMode?);
            var serviceDnsName = default(string);
            var scalingPolicies = default(IEnumerable<ScalingPolicyDescription>);
            var tagsRequiredToPlace = default(NodeTagsDescription);
            var tagsRequiredToRun = default(NodeTagsDescription);
            var instanceCount = default(int?);
            var minInstanceCount = default(int?);
            var minInstancePercentage = default(int?);
            var flags = default(int?);
            var instanceCloseDelayDurationSeconds = default(long?);
            var instanceLifecycleDescription = default(InstanceLifecycleDescription);
            var instanceRestartWaitDurationSeconds = default(long?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ApplicationName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationName = ApplicationNameConverter.Deserialize(reader);
                }
                else if (string.Compare("ServiceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = ServiceNameConverter.Deserialize(reader);
                }
                else if (string.Compare("ServiceTypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceTypeName = reader.ReadValueAsString();
                }
                else if (string.Compare("InitializationData", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    initializationData = ByteArrayConverter.Deserialize(reader);
                }
                else if (string.Compare("PartitionDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionDescription = PartitionSchemeDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("PlacementConstraints", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    placementConstraints = reader.ReadValueAsString();
                }
                else if (string.Compare("CorrelationScheme", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    correlationScheme = reader.ReadList(ServiceCorrelationDescriptionConverter.Deserialize);
                }
                else if (string.Compare("ServiceLoadMetrics", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceLoadMetrics = reader.ReadList(ServiceLoadMetricDescriptionConverter.Deserialize);
                }
                else if (string.Compare("ServicePlacementPolicies", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePlacementPolicies = reader.ReadList(ServicePlacementPolicyDescriptionConverter.Deserialize);
                }
                else if (string.Compare("DefaultMoveCost", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    defaultMoveCost = MoveCostConverter.Deserialize(reader);
                }
                else if (string.Compare("IsDefaultMoveCostSpecified", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isDefaultMoveCostSpecified = reader.ReadValueAsBool();
                }
                else if (string.Compare("ServicePackageActivationMode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePackageActivationMode = ServicePackageActivationModeConverter.Deserialize(reader);
                }
                else if (string.Compare("ServiceDnsName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceDnsName = reader.ReadValueAsString();
                }
                else if (string.Compare("ScalingPolicies", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scalingPolicies = reader.ReadList(ScalingPolicyDescriptionConverter.Deserialize);
                }
                else if (string.Compare("TagsRequiredToPlace", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tagsRequiredToPlace = NodeTagsDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("TagsRequiredToRun", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tagsRequiredToRun = NodeTagsDescriptionConverter.Deserialize(reader);
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
                else if (string.Compare("Flags", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    flags = reader.ReadValueAsInt();
                }
                else if (string.Compare("InstanceCloseDelayDurationSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceCloseDelayDurationSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("InstanceLifecycleDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceLifecycleDescription = InstanceLifecycleDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("InstanceRestartWaitDurationSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceRestartWaitDurationSeconds = reader.ReadValueAsLong();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new StatelessServiceDescription(
                applicationName: applicationName,
                serviceName: serviceName,
                serviceTypeName: serviceTypeName,
                initializationData: initializationData,
                partitionDescription: partitionDescription,
                placementConstraints: placementConstraints,
                correlationScheme: correlationScheme,
                serviceLoadMetrics: serviceLoadMetrics,
                servicePlacementPolicies: servicePlacementPolicies,
                defaultMoveCost: defaultMoveCost,
                isDefaultMoveCostSpecified: isDefaultMoveCostSpecified,
                servicePackageActivationMode: servicePackageActivationMode,
                serviceDnsName: serviceDnsName,
                scalingPolicies: scalingPolicies,
                tagsRequiredToPlace: tagsRequiredToPlace,
                tagsRequiredToRun: tagsRequiredToRun,
                instanceCount: instanceCount,
                minInstanceCount: minInstanceCount,
                minInstancePercentage: minInstancePercentage,
                flags: flags,
                instanceCloseDelayDurationSeconds: instanceCloseDelayDurationSeconds,
                instanceLifecycleDescription: instanceLifecycleDescription,
                instanceRestartWaitDurationSeconds: instanceRestartWaitDurationSeconds);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, StatelessServiceDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceKind, "ServiceKind", ServiceKindConverter.Serialize);
            writer.WriteProperty(obj.ServiceName, "ServiceName", ServiceNameConverter.Serialize);
            writer.WriteProperty(obj.ServiceTypeName, "ServiceTypeName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.PartitionDescription, "PartitionDescription", PartitionSchemeDescriptionConverter.Serialize);
            writer.WriteProperty(obj.InstanceCount, "InstanceCount", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.DefaultMoveCost, "DefaultMoveCost", MoveCostConverter.Serialize);
            writer.WriteProperty(obj.ServicePackageActivationMode, "ServicePackageActivationMode", ServicePackageActivationModeConverter.Serialize);
            if (obj.ApplicationName != null)
            {
                writer.WriteProperty(obj.ApplicationName, "ApplicationName", ApplicationNameConverter.Serialize);
            }

            if (obj.InitializationData != null)
            {
                writer.WriteProperty(obj.InitializationData, "InitializationData", ByteArrayConverter.Serialize);
            }

            if (obj.PlacementConstraints != null)
            {
                writer.WriteProperty(obj.PlacementConstraints, "PlacementConstraints", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CorrelationScheme != null)
            {
                writer.WriteEnumerableProperty(obj.CorrelationScheme, "CorrelationScheme", ServiceCorrelationDescriptionConverter.Serialize);
            }

            if (obj.ServiceLoadMetrics != null)
            {
                writer.WriteEnumerableProperty(obj.ServiceLoadMetrics, "ServiceLoadMetrics", ServiceLoadMetricDescriptionConverter.Serialize);
            }

            if (obj.ServicePlacementPolicies != null)
            {
                writer.WriteEnumerableProperty(obj.ServicePlacementPolicies, "ServicePlacementPolicies", ServicePlacementPolicyDescriptionConverter.Serialize);
            }

            if (obj.IsDefaultMoveCostSpecified != null)
            {
                writer.WriteProperty(obj.IsDefaultMoveCostSpecified, "IsDefaultMoveCostSpecified", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.ServiceDnsName != null)
            {
                writer.WriteProperty(obj.ServiceDnsName, "ServiceDnsName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ScalingPolicies != null)
            {
                writer.WriteEnumerableProperty(obj.ScalingPolicies, "ScalingPolicies", ScalingPolicyDescriptionConverter.Serialize);
            }

            if (obj.TagsRequiredToPlace != null)
            {
                writer.WriteProperty(obj.TagsRequiredToPlace, "TagsRequiredToPlace", NodeTagsDescriptionConverter.Serialize);
            }

            if (obj.TagsRequiredToRun != null)
            {
                writer.WriteProperty(obj.TagsRequiredToRun, "TagsRequiredToRun", NodeTagsDescriptionConverter.Serialize);
            }

            if (obj.MinInstanceCount != null)
            {
                writer.WriteProperty(obj.MinInstanceCount, "MinInstanceCount", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MinInstancePercentage != null)
            {
                writer.WriteProperty(obj.MinInstancePercentage, "MinInstancePercentage", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.Flags != null)
            {
                writer.WriteProperty(obj.Flags, "Flags", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.InstanceCloseDelayDurationSeconds != null)
            {
                writer.WriteProperty(obj.InstanceCloseDelayDurationSeconds, "InstanceCloseDelayDurationSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.InstanceLifecycleDescription != null)
            {
                writer.WriteProperty(obj.InstanceLifecycleDescription, "InstanceLifecycleDescription", InstanceLifecycleDescriptionConverter.Serialize);
            }

            if (obj.InstanceRestartWaitDurationSeconds != null)
            {
                writer.WriteProperty(obj.InstanceRestartWaitDurationSeconds, "InstanceRestartWaitDurationSeconds", JsonWriterExtensions.WriteLongValue);
            }

            writer.WriteEndObject();
        }
    }
}
