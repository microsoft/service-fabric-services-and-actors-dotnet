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
    /// Converter for <see cref="ProvisionFabricDescription" />.
    /// </summary>
    internal class ProvisionFabricDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ProvisionFabricDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ProvisionFabricDescription GetFromJsonProperties(JsonReader reader)
        {
            var codeFilePath = default(string);
            var clusterManifestFilePath = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("CodeFilePath", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codeFilePath = reader.ReadValueAsString();
                }
                else if (string.Compare("ClusterManifestFilePath", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterManifestFilePath = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ProvisionFabricDescription(
                codeFilePath: codeFilePath,
                clusterManifestFilePath: clusterManifestFilePath);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ProvisionFabricDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.CodeFilePath != null)
            {
                writer.WriteProperty(obj.CodeFilePath, "CodeFilePath", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ClusterManifestFilePath != null)
            {
                writer.WriteProperty(obj.ClusterManifestFilePath, "ClusterManifestFilePath", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
