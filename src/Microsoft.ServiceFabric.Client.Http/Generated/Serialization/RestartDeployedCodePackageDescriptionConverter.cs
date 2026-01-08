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
    /// Converter for <see cref="RestartDeployedCodePackageDescription" />.
    /// </summary>
    internal class RestartDeployedCodePackageDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RestartDeployedCodePackageDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RestartDeployedCodePackageDescription GetFromJsonProperties(JsonReader reader)
        {
            var serviceManifestName = default(string);
            var servicePackageActivationId = default(string);
            var codePackageName = default(string);
            var codePackageInstanceId = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ServiceManifestName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceManifestName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServicePackageActivationId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePackageActivationId = reader.ReadValueAsString();
                }
                else if (string.Compare("CodePackageName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codePackageName = reader.ReadValueAsString();
                }
                else if (string.Compare("CodePackageInstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codePackageInstanceId = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RestartDeployedCodePackageDescription(
                serviceManifestName: serviceManifestName,
                servicePackageActivationId: servicePackageActivationId,
                codePackageName: codePackageName,
                codePackageInstanceId: codePackageInstanceId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RestartDeployedCodePackageDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ServiceManifestName, "ServiceManifestName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.CodePackageName, "CodePackageName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.CodePackageInstanceId, "CodePackageInstanceId", JsonWriterExtensions.WriteStringValue);
            if (obj.ServicePackageActivationId != null)
            {
                writer.WriteProperty(obj.ServicePackageActivationId, "ServicePackageActivationId", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
