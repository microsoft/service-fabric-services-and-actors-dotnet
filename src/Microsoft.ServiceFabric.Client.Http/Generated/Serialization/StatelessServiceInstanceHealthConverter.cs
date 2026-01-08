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
    /// Converter for <see cref="StatelessServiceInstanceHealth" />.
    /// </summary>
    internal class StatelessServiceInstanceHealthConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceInstanceHealth Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceInstanceHealth GetFromJsonProperties(JsonReader reader)
        {
            var aggregatedHealthState = default(HealthState?);
            var healthEvents = default(IEnumerable<HealthEvent>);
            var unhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>);
            var healthStatistics = default(HealthStatistics);
            var partitionId = default(PartitionId);
            var instanceId = default(ReplicaId);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("AggregatedHealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    aggregatedHealthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("HealthEvents", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthEvents = reader.ReadList(HealthEventConverter.Deserialize);
                }
                else if (string.Compare("UnhealthyEvaluations", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    unhealthyEvaluations = reader.ReadList(HealthEvaluationWrapperConverter.Deserialize);
                }
                else if (string.Compare("HealthStatistics", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthStatistics = HealthStatisticsConverter.Deserialize(reader);
                }
                else if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = PartitionIdConverter.Deserialize(reader);
                }
                else if (string.Compare("InstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceId = ReplicaIdConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new StatelessServiceInstanceHealth(
                aggregatedHealthState: aggregatedHealthState,
                healthEvents: healthEvents,
                unhealthyEvaluations: unhealthyEvaluations,
                healthStatistics: healthStatistics,
                partitionId: partitionId,
                instanceId: instanceId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, StatelessServiceInstanceHealth obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceKind, "ServiceKind", ServiceKindConverter.Serialize);
            writer.WriteProperty(obj.AggregatedHealthState, "AggregatedHealthState", HealthStateConverter.Serialize);
            if (obj.HealthEvents != null)
            {
                writer.WriteEnumerableProperty(obj.HealthEvents, "HealthEvents", HealthEventConverter.Serialize);
            }

            if (obj.UnhealthyEvaluations != null)
            {
                writer.WriteEnumerableProperty(obj.UnhealthyEvaluations, "UnhealthyEvaluations", HealthEvaluationWrapperConverter.Serialize);
            }

            if (obj.HealthStatistics != null)
            {
                writer.WriteProperty(obj.HealthStatistics, "HealthStatistics", HealthStatisticsConverter.Serialize);
            }

            if (obj.PartitionId != null)
            {
                writer.WriteProperty(obj.PartitionId, "PartitionId", PartitionIdConverter.Serialize);
            }

            if (obj.InstanceId != null)
            {
                writer.WriteProperty(obj.InstanceId, "InstanceId", ReplicaIdConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
