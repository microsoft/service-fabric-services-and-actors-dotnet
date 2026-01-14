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
    /// Converter for <see cref="LoadedPartitionInformationQueryDescription" />.
    /// </summary>
    internal class LoadedPartitionInformationQueryDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static LoadedPartitionInformationQueryDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static LoadedPartitionInformationQueryDescription GetFromJsonProperties(JsonReader reader)
        {
            var metricName = default(string);
            var serviceName = default(string);
            var ordering = default(Ordering?);
            var maxResults = default(long?);
            var continuationToken = default(ContinuationToken);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("MetricName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    metricName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = reader.ReadValueAsString();
                }
                else if (string.Compare("Ordering", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ordering = OrderingConverter.Deserialize(reader);
                }
                else if (string.Compare("MaxResults", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxResults = reader.ReadValueAsLong();
                }
                else if (string.Compare("ContinuationToken", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continuationToken = ContinuationTokenConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new LoadedPartitionInformationQueryDescription(
                metricName: metricName,
                serviceName: serviceName,
                ordering: ordering,
                maxResults: maxResults,
                continuationToken: continuationToken);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, LoadedPartitionInformationQueryDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Ordering, "Ordering", OrderingConverter.Serialize);
            if (obj.MetricName != null)
            {
                writer.WriteProperty(obj.MetricName, "MetricName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceName != null)
            {
                writer.WriteProperty(obj.ServiceName, "ServiceName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.MaxResults != null)
            {
                writer.WriteProperty(obj.MaxResults, "MaxResults", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ContinuationToken != null)
            {
                writer.WriteProperty(obj.ContinuationToken, "ContinuationToken", ContinuationTokenConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
