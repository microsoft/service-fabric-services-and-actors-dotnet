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
    /// Converter for <see cref="RemoteReplicatorAcknowledgementDetail" />.
    /// </summary>
    internal class RemoteReplicatorAcknowledgementDetailConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RemoteReplicatorAcknowledgementDetail Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RemoteReplicatorAcknowledgementDetail GetFromJsonProperties(JsonReader reader)
        {
            var averageReceiveDuration = default(string);
            var averageApplyDuration = default(string);
            var notReceivedCount = default(string);
            var receivedAndNotAppliedCount = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("AverageReceiveDuration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    averageReceiveDuration = reader.ReadValueAsString();
                }
                else if (string.Compare("AverageApplyDuration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    averageApplyDuration = reader.ReadValueAsString();
                }
                else if (string.Compare("NotReceivedCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    notReceivedCount = reader.ReadValueAsString();
                }
                else if (string.Compare("ReceivedAndNotAppliedCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    receivedAndNotAppliedCount = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RemoteReplicatorAcknowledgementDetail(
                averageReceiveDuration: averageReceiveDuration,
                averageApplyDuration: averageApplyDuration,
                notReceivedCount: notReceivedCount,
                receivedAndNotAppliedCount: receivedAndNotAppliedCount);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RemoteReplicatorAcknowledgementDetail obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.AverageReceiveDuration != null)
            {
                writer.WriteProperty(obj.AverageReceiveDuration, "AverageReceiveDuration", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.AverageApplyDuration != null)
            {
                writer.WriteProperty(obj.AverageApplyDuration, "AverageApplyDuration", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NotReceivedCount != null)
            {
                writer.WriteProperty(obj.NotReceivedCount, "NotReceivedCount", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ReceivedAndNotAppliedCount != null)
            {
                writer.WriteProperty(obj.ReceivedAndNotAppliedCount, "ReceivedAndNotAppliedCount", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
