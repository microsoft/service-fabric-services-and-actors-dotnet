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
    /// Converter for <see cref="CreateComposeDeploymentDescription" />.
    /// </summary>
    internal class CreateComposeDeploymentDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static CreateComposeDeploymentDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static CreateComposeDeploymentDescription GetFromJsonProperties(JsonReader reader)
        {
            var deploymentName = default(string);
            var composeFileContent = default(string);
            var registryCredential = default(RegistryCredential);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("DeploymentName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deploymentName = reader.ReadValueAsString();
                }
                else if (string.Compare("ComposeFileContent", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    composeFileContent = reader.ReadValueAsString();
                }
                else if (string.Compare("RegistryCredential", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    registryCredential = RegistryCredentialConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new CreateComposeDeploymentDescription(
                deploymentName: deploymentName,
                composeFileContent: composeFileContent,
                registryCredential: registryCredential);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, CreateComposeDeploymentDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.DeploymentName, "DeploymentName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ComposeFileContent, "ComposeFileContent", JsonWriterExtensions.WriteStringValue);
            if (obj.RegistryCredential != null)
            {
                writer.WriteProperty(obj.RegistryCredential, "RegistryCredential", RegistryCredentialConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
