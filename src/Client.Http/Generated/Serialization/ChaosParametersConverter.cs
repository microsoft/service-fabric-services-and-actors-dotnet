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
    /// Converter for <see cref="ChaosParameters" />.
    /// </summary>
    internal class ChaosParametersConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosParameters Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ChaosParameters GetFromJsonProperties(JsonReader reader)
        {
            var timeToRunInSeconds = default(string);
            var maxClusterStabilizationTimeoutInSeconds = default(long?);
            var maxConcurrentFaults = default(long?);
            var enableMoveReplicaFaults = default(bool?);
            var waitTimeBetweenFaultsInSeconds = default(long?);
            var waitTimeBetweenIterationsInSeconds = default(long?);
            var clusterHealthPolicy = default(ClusterHealthPolicy);
            var context = default(ChaosContext);
            var chaosTargetFilter = default(ChaosTargetFilter);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("TimeToRunInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeToRunInSeconds = reader.ReadValueAsString();
                }
                else if (string.Compare("MaxClusterStabilizationTimeoutInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxClusterStabilizationTimeoutInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("MaxConcurrentFaults", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxConcurrentFaults = reader.ReadValueAsLong();
                }
                else if (string.Compare("EnableMoveReplicaFaults", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    enableMoveReplicaFaults = reader.ReadValueAsBool();
                }
                else if (string.Compare("WaitTimeBetweenFaultsInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    waitTimeBetweenFaultsInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("WaitTimeBetweenIterationsInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    waitTimeBetweenIterationsInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("ClusterHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterHealthPolicy = ClusterHealthPolicyConverter.Deserialize(reader);
                }
                else if (string.Compare("Context", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    context = ChaosContextConverter.Deserialize(reader);
                }
                else if (string.Compare("ChaosTargetFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    chaosTargetFilter = ChaosTargetFilterConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ChaosParameters(
                timeToRunInSeconds: timeToRunInSeconds,
                maxClusterStabilizationTimeoutInSeconds: maxClusterStabilizationTimeoutInSeconds,
                maxConcurrentFaults: maxConcurrentFaults,
                enableMoveReplicaFaults: enableMoveReplicaFaults,
                waitTimeBetweenFaultsInSeconds: waitTimeBetweenFaultsInSeconds,
                waitTimeBetweenIterationsInSeconds: waitTimeBetweenIterationsInSeconds,
                clusterHealthPolicy: clusterHealthPolicy,
                context: context,
                chaosTargetFilter: chaosTargetFilter);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ChaosParameters obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.TimeToRunInSeconds != null)
            {
                writer.WriteProperty(obj.TimeToRunInSeconds, "TimeToRunInSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.MaxClusterStabilizationTimeoutInSeconds != null)
            {
                writer.WriteProperty(obj.MaxClusterStabilizationTimeoutInSeconds, "MaxClusterStabilizationTimeoutInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.MaxConcurrentFaults != null)
            {
                writer.WriteProperty(obj.MaxConcurrentFaults, "MaxConcurrentFaults", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.EnableMoveReplicaFaults != null)
            {
                writer.WriteProperty(obj.EnableMoveReplicaFaults, "EnableMoveReplicaFaults", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.WaitTimeBetweenFaultsInSeconds != null)
            {
                writer.WriteProperty(obj.WaitTimeBetweenFaultsInSeconds, "WaitTimeBetweenFaultsInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.WaitTimeBetweenIterationsInSeconds != null)
            {
                writer.WriteProperty(obj.WaitTimeBetweenIterationsInSeconds, "WaitTimeBetweenIterationsInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ClusterHealthPolicy != null)
            {
                writer.WriteProperty(obj.ClusterHealthPolicy, "ClusterHealthPolicy", ClusterHealthPolicyConverter.Serialize);
            }

            if (obj.Context != null)
            {
                writer.WriteProperty(obj.Context, "Context", ChaosContextConverter.Serialize);
            }

            if (obj.ChaosTargetFilter != null)
            {
                writer.WriteProperty(obj.ChaosTargetFilter, "ChaosTargetFilter", ChaosTargetFilterConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
