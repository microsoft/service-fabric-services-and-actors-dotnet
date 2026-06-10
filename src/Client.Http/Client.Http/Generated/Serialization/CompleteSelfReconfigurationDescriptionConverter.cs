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
    /// Converter for <see cref="CompleteSelfReconfigurationDescription" />.
    /// </summary>
    internal class CompleteSelfReconfigurationDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static CompleteSelfReconfigurationDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static CompleteSelfReconfigurationDescription GetFromJsonProperties(JsonReader reader)
        {
            var partitionId = default(Guid?);
            var requestSequenceNumber = default(long?);
            var requestGenerationNumber = default(long?);
            var reportId = default(long?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PartitionId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("RequestSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    requestSequenceNumber = reader.ReadValueAsLong();
                }
                else if (string.Compare("RequestGenerationNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    requestGenerationNumber = reader.ReadValueAsLong();
                }
                else if (string.Compare("ReportId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    reportId = reader.ReadValueAsLong();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new CompleteSelfReconfigurationDescription(
                partitionId: partitionId,
                requestSequenceNumber: requestSequenceNumber,
                requestGenerationNumber: requestGenerationNumber,
                reportId: reportId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, CompleteSelfReconfigurationDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.PartitionId, "PartitionId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.RequestSequenceNumber, "RequestSequenceNumber", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.RequestGenerationNumber, "RequestGenerationNumber", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.ReportId, "ReportId", JsonWriterExtensions.WriteLongValue);
            writer.WriteEndObject();
        }
    }
}
