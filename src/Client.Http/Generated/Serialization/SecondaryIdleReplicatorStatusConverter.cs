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
    /// Converter for <see cref="SecondaryIdleReplicatorStatus" />.
    /// </summary>
    internal class SecondaryIdleReplicatorStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static SecondaryIdleReplicatorStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static SecondaryIdleReplicatorStatus GetFromJsonProperties(JsonReader reader)
        {
            var replicationQueueStatus = default(ReplicatorQueueStatus);
            var lastReplicationOperationReceivedTimeUtc = default(DateTime?);
            var isInBuild = default(bool?);
            var copyQueueStatus = default(ReplicatorQueueStatus);
            var lastCopyOperationReceivedTimeUtc = default(DateTime?);
            var lastAcknowledgementSentTimeUtc = default(DateTime?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ReplicationQueueStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicationQueueStatus = ReplicatorQueueStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("LastReplicationOperationReceivedTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastReplicationOperationReceivedTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("IsInBuild", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isInBuild = reader.ReadValueAsBool();
                }
                else if (string.Compare("CopyQueueStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyQueueStatus = ReplicatorQueueStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("LastCopyOperationReceivedTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastCopyOperationReceivedTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastAcknowledgementSentTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastAcknowledgementSentTimeUtc = reader.ReadValueAsDateTime();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new SecondaryIdleReplicatorStatus(
                replicationQueueStatus: replicationQueueStatus,
                lastReplicationOperationReceivedTimeUtc: lastReplicationOperationReceivedTimeUtc,
                isInBuild: isInBuild,
                copyQueueStatus: copyQueueStatus,
                lastCopyOperationReceivedTimeUtc: lastCopyOperationReceivedTimeUtc,
                lastAcknowledgementSentTimeUtc: lastAcknowledgementSentTimeUtc);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, SecondaryIdleReplicatorStatus obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ReplicaRoleConverter.Serialize);
            if (obj.ReplicationQueueStatus != null)
            {
                writer.WriteProperty(obj.ReplicationQueueStatus, "ReplicationQueueStatus", ReplicatorQueueStatusConverter.Serialize);
            }

            if (obj.LastReplicationOperationReceivedTimeUtc != null)
            {
                writer.WriteProperty(obj.LastReplicationOperationReceivedTimeUtc, "LastReplicationOperationReceivedTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.IsInBuild != null)
            {
                writer.WriteProperty(obj.IsInBuild, "IsInBuild", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.CopyQueueStatus != null)
            {
                writer.WriteProperty(obj.CopyQueueStatus, "CopyQueueStatus", ReplicatorQueueStatusConverter.Serialize);
            }

            if (obj.LastCopyOperationReceivedTimeUtc != null)
            {
                writer.WriteProperty(obj.LastCopyOperationReceivedTimeUtc, "LastCopyOperationReceivedTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastAcknowledgementSentTimeUtc != null)
            {
                writer.WriteProperty(obj.LastAcknowledgementSentTimeUtc, "LastAcknowledgementSentTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            writer.WriteEndObject();
        }
    }
}
