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
    /// Converter for <see cref="StatelessServiceInstanceInfo" />.
    /// </summary>
    internal class StatelessServiceInstanceInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceInstanceInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static StatelessServiceInstanceInfo GetFromJsonProperties(JsonReader reader)
        {
            var replicaStatus = default(ReplicaStatus?);
            var healthState = default(HealthState?);
            var nodeName = default(NodeName);
            var address = default(string);
            var lastInBuildDurationInSeconds = default(string);
            var instanceId = default(ReplicaId);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ReplicaStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaStatus = ReplicaStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("NodeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeName = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("Address", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    address = reader.ReadValueAsString();
                }
                else if (string.Compare("LastInBuildDurationInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastInBuildDurationInSeconds = reader.ReadValueAsString();
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

            return new StatelessServiceInstanceInfo(
                replicaStatus: replicaStatus,
                healthState: healthState,
                nodeName: nodeName,
                address: address,
                lastInBuildDurationInSeconds: lastInBuildDurationInSeconds,
                instanceId: instanceId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, StatelessServiceInstanceInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceKind, "ServiceKind", ServiceKindConverter.Serialize);
            writer.WriteProperty(obj.ReplicaStatus, "ReplicaStatus", ReplicaStatusConverter.Serialize);
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            if (obj.NodeName != null)
            {
                writer.WriteProperty(obj.NodeName, "NodeName", NodeNameConverter.Serialize);
            }

            if (obj.Address != null)
            {
                writer.WriteProperty(obj.Address, "Address", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LastInBuildDurationInSeconds != null)
            {
                writer.WriteProperty(obj.LastInBuildDurationInSeconds, "LastInBuildDurationInSeconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.InstanceId != null)
            {
                writer.WriteProperty(obj.InstanceId, "InstanceId", ReplicaIdConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
