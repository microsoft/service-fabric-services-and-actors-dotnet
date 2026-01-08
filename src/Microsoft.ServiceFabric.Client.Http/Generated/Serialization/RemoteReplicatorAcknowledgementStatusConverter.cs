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
    /// Converter for <see cref="RemoteReplicatorAcknowledgementStatus" />.
    /// </summary>
    internal class RemoteReplicatorAcknowledgementStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RemoteReplicatorAcknowledgementStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RemoteReplicatorAcknowledgementStatus GetFromJsonProperties(JsonReader reader)
        {
            var replicationStreamAcknowledgementDetail = default(RemoteReplicatorAcknowledgementDetail);
            var copyStreamAcknowledgementDetail = default(RemoteReplicatorAcknowledgementDetail);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ReplicationStreamAcknowledgementDetail", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicationStreamAcknowledgementDetail = RemoteReplicatorAcknowledgementDetailConverter.Deserialize(reader);
                }
                else if (string.Compare("CopyStreamAcknowledgementDetail", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyStreamAcknowledgementDetail = RemoteReplicatorAcknowledgementDetailConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RemoteReplicatorAcknowledgementStatus(
                replicationStreamAcknowledgementDetail: replicationStreamAcknowledgementDetail,
                copyStreamAcknowledgementDetail: copyStreamAcknowledgementDetail);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RemoteReplicatorAcknowledgementStatus obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ReplicationStreamAcknowledgementDetail != null)
            {
                writer.WriteProperty(obj.ReplicationStreamAcknowledgementDetail, "ReplicationStreamAcknowledgementDetail", RemoteReplicatorAcknowledgementDetailConverter.Serialize);
            }

            if (obj.CopyStreamAcknowledgementDetail != null)
            {
                writer.WriteProperty(obj.CopyStreamAcknowledgementDetail, "CopyStreamAcknowledgementDetail", RemoteReplicatorAcknowledgementDetailConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
