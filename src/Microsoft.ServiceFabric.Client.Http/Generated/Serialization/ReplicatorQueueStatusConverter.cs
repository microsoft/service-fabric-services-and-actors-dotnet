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
    /// Converter for <see cref="ReplicatorQueueStatus" />.
    /// </summary>
    internal class ReplicatorQueueStatusConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicatorQueueStatus Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicatorQueueStatus GetFromJsonProperties(JsonReader reader)
        {
            var queueUtilizationPercentage = default(int?);
            var queueMemorySize = default(string);
            var firstSequenceNumber = default(string);
            var completedSequenceNumber = default(string);
            var committedSequenceNumber = default(string);
            var lastSequenceNumber = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("QueueUtilizationPercentage", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    queueUtilizationPercentage = reader.ReadValueAsInt();
                }
                else if (string.Compare("QueueMemorySize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    queueMemorySize = reader.ReadValueAsString();
                }
                else if (string.Compare("FirstSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    firstSequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("CompletedSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    completedSequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("CommittedSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    committedSequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("LastSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastSequenceNumber = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ReplicatorQueueStatus(
                queueUtilizationPercentage: queueUtilizationPercentage,
                queueMemorySize: queueMemorySize,
                firstSequenceNumber: firstSequenceNumber,
                completedSequenceNumber: completedSequenceNumber,
                committedSequenceNumber: committedSequenceNumber,
                lastSequenceNumber: lastSequenceNumber);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ReplicatorQueueStatus obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.QueueUtilizationPercentage != null)
            {
                writer.WriteProperty(obj.QueueUtilizationPercentage, "QueueUtilizationPercentage", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.QueueMemorySize != null)
            {
                writer.WriteProperty(obj.QueueMemorySize, "QueueMemorySize", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FirstSequenceNumber != null)
            {
                writer.WriteProperty(obj.FirstSequenceNumber, "FirstSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CompletedSequenceNumber != null)
            {
                writer.WriteProperty(obj.CompletedSequenceNumber, "CompletedSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CommittedSequenceNumber != null)
            {
                writer.WriteProperty(obj.CommittedSequenceNumber, "CommittedSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LastSequenceNumber != null)
            {
                writer.WriteProperty(obj.LastSequenceNumber, "LastSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
