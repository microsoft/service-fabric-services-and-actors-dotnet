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
    /// Converter for <see cref="RegistryCredential" />.
    /// </summary>
    internal class RegistryCredentialConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RegistryCredential Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RegistryCredential GetFromJsonProperties(JsonReader reader)
        {
            var registryUserName = default(string);
            var registryPassword = default(string);
            var passwordEncrypted = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("RegistryUserName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    registryUserName = reader.ReadValueAsString();
                }
                else if (string.Compare("RegistryPassword", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    registryPassword = reader.ReadValueAsString();
                }
                else if (string.Compare("PasswordEncrypted", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    passwordEncrypted = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RegistryCredential(
                registryUserName: registryUserName,
                registryPassword: registryPassword,
                passwordEncrypted: passwordEncrypted);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RegistryCredential obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.RegistryUserName != null)
            {
                writer.WriteProperty(obj.RegistryUserName, "RegistryUserName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.RegistryPassword != null)
            {
                writer.WriteProperty(obj.RegistryPassword, "RegistryPassword", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PasswordEncrypted != null)
            {
                writer.WriteProperty(obj.PasswordEncrypted, "PasswordEncrypted", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
