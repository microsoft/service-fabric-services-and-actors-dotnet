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
    /// Converter for <see cref="ReplicaMetricLoadDescription" />.
    /// </summary>
    internal class ReplicaMetricLoadDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicaMetricLoadDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicaMetricLoadDescription GetFromJsonProperties(JsonReader reader)
        {
            var nodeName = default(string);
            var replicaOrInstanceLoadEntries = default(IEnumerable<MetricLoadDescription>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("NodeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeName = reader.ReadValueAsString();
                }
                else if (string.Compare("ReplicaOrInstanceLoadEntries", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaOrInstanceLoadEntries = reader.ReadList(MetricLoadDescriptionConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ReplicaMetricLoadDescription(
                nodeName: nodeName,
                replicaOrInstanceLoadEntries: replicaOrInstanceLoadEntries);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ReplicaMetricLoadDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.NodeName != null)
            {
                writer.WriteProperty(obj.NodeName, "NodeName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ReplicaOrInstanceLoadEntries != null)
            {
                writer.WriteEnumerableProperty(obj.ReplicaOrInstanceLoadEntries, "ReplicaOrInstanceLoadEntries", MetricLoadDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
