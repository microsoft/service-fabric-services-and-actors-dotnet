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
    /// Converter for <see cref="ClusterHealthChunk" />.
    /// </summary>
    internal class ClusterHealthChunkConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterHealthChunk Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterHealthChunk GetFromJsonProperties(JsonReader reader)
        {
            var healthState = default(HealthState?);
            var nodeHealthStateChunks = default(NodeHealthStateChunkList);
            var applicationHealthStateChunks = default(ApplicationHealthStateChunkList);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("NodeHealthStateChunks", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeHealthStateChunks = NodeHealthStateChunkListConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationHealthStateChunks", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthStateChunks = ApplicationHealthStateChunkListConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ClusterHealthChunk(
                healthState: healthState,
                nodeHealthStateChunks: nodeHealthStateChunks,
                applicationHealthStateChunks: applicationHealthStateChunks);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ClusterHealthChunk obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            if (obj.NodeHealthStateChunks != null)
            {
                writer.WriteProperty(obj.NodeHealthStateChunks, "NodeHealthStateChunks", NodeHealthStateChunkListConverter.Serialize);
            }

            if (obj.ApplicationHealthStateChunks != null)
            {
                writer.WriteProperty(obj.ApplicationHealthStateChunks, "ApplicationHealthStateChunks", ApplicationHealthStateChunkListConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
