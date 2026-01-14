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
    /// Converter for <see cref="ChaosStartedEvent" />.
    /// </summary>
    internal class ChaosStartedEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosStartedEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosStartedEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var maxConcurrentFaults = default(long?);
            var timeToRunInSeconds = default(double?);
            var maxClusterStabilizationTimeoutInSeconds = default(double?);
            var waitTimeBetweenIterationsInSeconds = default(double?);
            var waitTimeBetweenFaultsInSeconds = default(double?);
            var moveReplicaFaultEnabled = default(bool?);
            var includedNodeTypeList = default(string);
            var includedApplicationList = default(string);
            var clusterHealthPolicy = default(string);
            var chaosContext = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("EventInstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    eventInstanceId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("Category", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    category = reader.ReadValueAsString();
                }
                else if (string.Compare("TimeStamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeStamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("HasCorrelatedEvents", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hasCorrelatedEvents = reader.ReadValueAsBool();
                }
                else if (string.Compare("MaxConcurrentFaults", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxConcurrentFaults = reader.ReadValueAsLong();
                }
                else if (string.Compare("TimeToRunInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeToRunInSeconds = reader.ReadValueAsDouble();
                }
                else if (string.Compare("MaxClusterStabilizationTimeoutInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxClusterStabilizationTimeoutInSeconds = reader.ReadValueAsDouble();
                }
                else if (string.Compare("WaitTimeBetweenIterationsInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    waitTimeBetweenIterationsInSeconds = reader.ReadValueAsDouble();
                }
                else if (string.Compare("WaitTimeBetweenFaultsInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    waitTimeBetweenFaultsInSeconds = reader.ReadValueAsDouble();
                }
                else if (string.Compare("MoveReplicaFaultEnabled", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    moveReplicaFaultEnabled = reader.ReadValueAsBool();
                }
                else if (string.Compare("IncludedNodeTypeList", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    includedNodeTypeList = reader.ReadValueAsString();
                }
                else if (string.Compare("IncludedApplicationList", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    includedApplicationList = reader.ReadValueAsString();
                }
                else if (string.Compare("ClusterHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterHealthPolicy = reader.ReadValueAsString();
                }
                else if (string.Compare("ChaosContext", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    chaosContext = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ChaosStartedEvent(
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                maxConcurrentFaults: maxConcurrentFaults,
                timeToRunInSeconds: timeToRunInSeconds,
                maxClusterStabilizationTimeoutInSeconds: maxClusterStabilizationTimeoutInSeconds,
                waitTimeBetweenIterationsInSeconds: waitTimeBetweenIterationsInSeconds,
                waitTimeBetweenFaultsInSeconds: waitTimeBetweenFaultsInSeconds,
                moveReplicaFaultEnabled: moveReplicaFaultEnabled,
                includedNodeTypeList: includedNodeTypeList,
                includedApplicationList: includedApplicationList,
                clusterHealthPolicy: clusterHealthPolicy,
                chaosContext: chaosContext);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ChaosStartedEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.MaxConcurrentFaults, "MaxConcurrentFaults", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.TimeToRunInSeconds, "TimeToRunInSeconds", JsonWriterExtensions.WriteDoubleValue);
            writer.WriteProperty(obj.MaxClusterStabilizationTimeoutInSeconds, "MaxClusterStabilizationTimeoutInSeconds", JsonWriterExtensions.WriteDoubleValue);
            writer.WriteProperty(obj.WaitTimeBetweenIterationsInSeconds, "WaitTimeBetweenIterationsInSeconds", JsonWriterExtensions.WriteDoubleValue);
            writer.WriteProperty(obj.WaitTimeBetweenFaultsInSeconds, "WaitTimeBetweenFaultsInSeconds", JsonWriterExtensions.WriteDoubleValue);
            writer.WriteProperty(obj.MoveReplicaFaultEnabled, "MoveReplicaFaultEnabled", JsonWriterExtensions.WriteBoolValue);
            writer.WriteProperty(obj.IncludedNodeTypeList, "IncludedNodeTypeList", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.IncludedApplicationList, "IncludedApplicationList", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ClusterHealthPolicy, "ClusterHealthPolicy", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ChaosContext, "ChaosContext", JsonWriterExtensions.WriteStringValue);
            if (obj.Category != null)
            {
                writer.WriteProperty(obj.Category, "Category", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HasCorrelatedEvents != null)
            {
                writer.WriteProperty(obj.HasCorrelatedEvents, "HasCorrelatedEvents", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
