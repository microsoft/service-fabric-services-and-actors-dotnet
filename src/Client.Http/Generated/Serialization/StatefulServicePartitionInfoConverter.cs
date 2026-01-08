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
    /// Converter for <see cref="StatefulServicePartitionInfo" />.
    /// </summary>
    internal class StatefulServicePartitionInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static StatefulServicePartitionInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static StatefulServicePartitionInfo GetFromJsonProperties(JsonReader reader)
        {
            var healthState = default(HealthState?);
            var partitionStatus = default(ServicePartitionStatus?);
            var partitionInformation = default(PartitionInformation);
            var targetReplicaSetSize = default(long?);
            var minReplicaSetSize = default(long?);
            var auxiliaryReplicaCount = default(long?);
            var lastQuorumLossDuration = default(TimeSpan?);
            var primaryEpoch = default(Epoch);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("PartitionStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionStatus = ServicePartitionStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("PartitionInformation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionInformation = PartitionInformationConverter.Deserialize(reader);
                }
                else if (string.Compare("TargetReplicaSetSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    targetReplicaSetSize = reader.ReadValueAsLong();
                }
                else if (string.Compare("MinReplicaSetSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minReplicaSetSize = reader.ReadValueAsLong();
                }
                else if (string.Compare("AuxiliaryReplicaCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    auxiliaryReplicaCount = reader.ReadValueAsLong();
                }
                else if (string.Compare("LastQuorumLossDuration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastQuorumLossDuration = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("PrimaryEpoch", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryEpoch = EpochConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new StatefulServicePartitionInfo(
                healthState: healthState,
                partitionStatus: partitionStatus,
                partitionInformation: partitionInformation,
                targetReplicaSetSize: targetReplicaSetSize,
                minReplicaSetSize: minReplicaSetSize,
                auxiliaryReplicaCount: auxiliaryReplicaCount,
                lastQuorumLossDuration: lastQuorumLossDuration,
                primaryEpoch: primaryEpoch);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, StatefulServicePartitionInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceKind, "ServiceKind", ServiceKindConverter.Serialize);
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            writer.WriteProperty(obj.PartitionStatus, "PartitionStatus", ServicePartitionStatusConverter.Serialize);
            if (obj.PartitionInformation != null)
            {
                writer.WriteProperty(obj.PartitionInformation, "PartitionInformation", PartitionInformationConverter.Serialize);
            }

            if (obj.TargetReplicaSetSize != null)
            {
                writer.WriteProperty(obj.TargetReplicaSetSize, "TargetReplicaSetSize", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.MinReplicaSetSize != null)
            {
                writer.WriteProperty(obj.MinReplicaSetSize, "MinReplicaSetSize", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.AuxiliaryReplicaCount != null)
            {
                writer.WriteProperty(obj.AuxiliaryReplicaCount, "AuxiliaryReplicaCount", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.LastQuorumLossDuration != null)
            {
                writer.WriteProperty(obj.LastQuorumLossDuration, "LastQuorumLossDuration", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.PrimaryEpoch != null)
            {
                writer.WriteProperty(obj.PrimaryEpoch, "PrimaryEpoch", EpochConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
