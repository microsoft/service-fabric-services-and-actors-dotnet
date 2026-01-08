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
    /// Converter for <see cref="PrimaryReplicatorStatus" />.
    /// </summary>
    internal class PrimaryReplicatorStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PrimaryReplicatorStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PrimaryReplicatorStatus GetFromJsonProperties(JsonReader reader)
        {
            var replicationQueueStatus = default(ReplicatorQueueStatus);
            var remoteReplicators = default(IEnumerable<RemoteReplicatorStatus>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ReplicationQueueStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicationQueueStatus = ReplicatorQueueStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("RemoteReplicators", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    remoteReplicators = reader.ReadList(RemoteReplicatorStatusConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PrimaryReplicatorStatus(
                replicationQueueStatus: replicationQueueStatus,
                remoteReplicators: remoteReplicators);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PrimaryReplicatorStatus obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ReplicaRoleConverter.Serialize);
            if (obj.ReplicationQueueStatus != null)
            {
                writer.WriteProperty(obj.ReplicationQueueStatus, "ReplicationQueueStatus", ReplicatorQueueStatusConverter.Serialize);
            }

            if (obj.RemoteReplicators != null)
            {
                writer.WriteEnumerableProperty(obj.RemoteReplicators, "RemoteReplicators", RemoteReplicatorStatusConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
