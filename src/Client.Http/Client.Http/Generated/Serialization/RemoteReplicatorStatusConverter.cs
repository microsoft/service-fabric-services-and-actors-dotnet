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
    /// Converter for <see cref="RemoteReplicatorStatus" />.
    /// </summary>
    internal class RemoteReplicatorStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RemoteReplicatorStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RemoteReplicatorStatus GetFromJsonProperties(JsonReader reader)
        {
            var replicaId = default(ReplicaId);
            var lastAcknowledgementProcessedTimeUtc = default(DateTime?);
            var lastReceivedReplicationSequenceNumber = default(string);
            var lastAppliedReplicationSequenceNumber = default(string);
            var isInBuild = default(bool?);
            var lastReceivedCopySequenceNumber = default(string);
            var lastAppliedCopySequenceNumber = default(string);
            var remoteReplicatorAcknowledgementStatus = default(RemoteReplicatorAcknowledgementStatus);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ReplicaId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaId = ReplicaIdConverter.Deserialize(reader);
                }
                else if (string.Compare("LastAcknowledgementProcessedTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastAcknowledgementProcessedTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastReceivedReplicationSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastReceivedReplicationSequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("LastAppliedReplicationSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastAppliedReplicationSequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("IsInBuild", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isInBuild = reader.ReadValueAsBool();
                }
                else if (string.Compare("LastReceivedCopySequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastReceivedCopySequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("LastAppliedCopySequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastAppliedCopySequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("RemoteReplicatorAcknowledgementStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    remoteReplicatorAcknowledgementStatus = RemoteReplicatorAcknowledgementStatusConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RemoteReplicatorStatus(
                replicaId: replicaId,
                lastAcknowledgementProcessedTimeUtc: lastAcknowledgementProcessedTimeUtc,
                lastReceivedReplicationSequenceNumber: lastReceivedReplicationSequenceNumber,
                lastAppliedReplicationSequenceNumber: lastAppliedReplicationSequenceNumber,
                isInBuild: isInBuild,
                lastReceivedCopySequenceNumber: lastReceivedCopySequenceNumber,
                lastAppliedCopySequenceNumber: lastAppliedCopySequenceNumber,
                remoteReplicatorAcknowledgementStatus: remoteReplicatorAcknowledgementStatus);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RemoteReplicatorStatus obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ReplicaId != null)
            {
                writer.WriteProperty(obj.ReplicaId, "ReplicaId", ReplicaIdConverter.Serialize);
            }

            if (obj.LastAcknowledgementProcessedTimeUtc != null)
            {
                writer.WriteProperty(obj.LastAcknowledgementProcessedTimeUtc, "LastAcknowledgementProcessedTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastReceivedReplicationSequenceNumber != null)
            {
                writer.WriteProperty(obj.LastReceivedReplicationSequenceNumber, "LastReceivedReplicationSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LastAppliedReplicationSequenceNumber != null)
            {
                writer.WriteProperty(obj.LastAppliedReplicationSequenceNumber, "LastAppliedReplicationSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.IsInBuild != null)
            {
                writer.WriteProperty(obj.IsInBuild, "IsInBuild", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.LastReceivedCopySequenceNumber != null)
            {
                writer.WriteProperty(obj.LastReceivedCopySequenceNumber, "LastReceivedCopySequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LastAppliedCopySequenceNumber != null)
            {
                writer.WriteProperty(obj.LastAppliedCopySequenceNumber, "LastAppliedCopySequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.RemoteReplicatorAcknowledgementStatus != null)
            {
                writer.WriteProperty(obj.RemoteReplicatorAcknowledgementStatus, "RemoteReplicatorAcknowledgementStatus", RemoteReplicatorAcknowledgementStatusConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
