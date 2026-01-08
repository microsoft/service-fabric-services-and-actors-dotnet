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
    /// Converter for <see cref="DeployedServicePackageHealthStateChunk" />.
    /// </summary>
    internal class DeployedServicePackageHealthStateChunkConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedServicePackageHealthStateChunk Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeployedServicePackageHealthStateChunk GetFromJsonProperties(JsonReader reader)
        {
            var healthState = default(HealthState?);
            var serviceManifestName = default(string);
            var servicePackageActivationId = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("ServiceManifestName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceManifestName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServicePackageActivationId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePackageActivationId = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeployedServicePackageHealthStateChunk(
                healthState: healthState,
                serviceManifestName: serviceManifestName,
                servicePackageActivationId: servicePackageActivationId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeployedServicePackageHealthStateChunk obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            if (obj.ServiceManifestName != null)
            {
                writer.WriteProperty(obj.ServiceManifestName, "ServiceManifestName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServicePackageActivationId != null)
            {
                writer.WriteProperty(obj.ServicePackageActivationId, "ServicePackageActivationId", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
