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
    /// Converter for <see cref="ApplicationHealthStateChunk" />.
    /// </summary>
    internal class ApplicationHealthStateChunkConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationHealthStateChunk Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationHealthStateChunk GetFromJsonProperties(JsonReader reader)
        {
            var healthState = default(HealthState?);
            var applicationName = default(ApplicationName);
            var applicationTypeName = default(string);
            var serviceHealthStateChunks = default(ServiceHealthStateChunkList);
            var deployedApplicationHealthStateChunks = default(DeployedApplicationHealthStateChunkList);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationName = ApplicationNameConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationTypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceHealthStateChunks", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceHealthStateChunks = ServiceHealthStateChunkListConverter.Deserialize(reader);
                }
                else if (string.Compare("DeployedApplicationHealthStateChunks", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deployedApplicationHealthStateChunks = DeployedApplicationHealthStateChunkListConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationHealthStateChunk(
                healthState: healthState,
                applicationName: applicationName,
                applicationTypeName: applicationTypeName,
                serviceHealthStateChunks: serviceHealthStateChunks,
                deployedApplicationHealthStateChunks: deployedApplicationHealthStateChunks);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationHealthStateChunk obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            if (obj.ApplicationName != null)
            {
                writer.WriteProperty(obj.ApplicationName, "ApplicationName", ApplicationNameConverter.Serialize);
            }

            if (obj.ApplicationTypeName != null)
            {
                writer.WriteProperty(obj.ApplicationTypeName, "ApplicationTypeName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceHealthStateChunks != null)
            {
                writer.WriteProperty(obj.ServiceHealthStateChunks, "ServiceHealthStateChunks", ServiceHealthStateChunkListConverter.Serialize);
            }

            if (obj.DeployedApplicationHealthStateChunks != null)
            {
                writer.WriteProperty(obj.DeployedApplicationHealthStateChunks, "DeployedApplicationHealthStateChunks", DeployedApplicationHealthStateChunkListConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
