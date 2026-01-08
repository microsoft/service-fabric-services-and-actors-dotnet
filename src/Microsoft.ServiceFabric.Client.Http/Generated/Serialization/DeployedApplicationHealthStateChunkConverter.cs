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
    /// Converter for <see cref="DeployedApplicationHealthStateChunk" />.
    /// </summary>
    internal class DeployedApplicationHealthStateChunkConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedApplicationHealthStateChunk Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedApplicationHealthStateChunk GetFromJsonProperties(JsonReader reader)
        {
            var healthState = default(HealthState?);
            var nodeName = default(string);
            var deployedServicePackageHealthStateChunks = default(DeployedServicePackageHealthStateChunkList);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("NodeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeName = reader.ReadValueAsString();
                }
                else if (string.Compare("DeployedServicePackageHealthStateChunks", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deployedServicePackageHealthStateChunks = DeployedServicePackageHealthStateChunkListConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeployedApplicationHealthStateChunk(
                healthState: healthState,
                nodeName: nodeName,
                deployedServicePackageHealthStateChunks: deployedServicePackageHealthStateChunks);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployedApplicationHealthStateChunk obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            if (obj.NodeName != null)
            {
                writer.WriteProperty(obj.NodeName, "NodeName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.DeployedServicePackageHealthStateChunks != null)
            {
                writer.WriteProperty(obj.DeployedServicePackageHealthStateChunks, "DeployedServicePackageHealthStateChunks", DeployedServicePackageHealthStateChunkListConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
