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
    /// Converter for <see cref="ApplicationDescription" />.
    /// </summary>
    internal class ApplicationDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationDescription GetFromJsonProperties(JsonReader reader)
        {
            var name = default(ApplicationName);
            var typeName = default(string);
            var typeVersion = default(string);
            var parameters = default(IReadOnlyDictionary<string, string>);
            var applicationCapacity = default(ApplicationCapacityDescription);
            var managedApplicationIdentity = default(ManagedApplicationIdentityDescription);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = ApplicationNameConverter.Deserialize(reader);
                }
                else if (string.Compare("TypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    typeName = reader.ReadValueAsString();
                }
                else if (string.Compare("TypeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    typeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("ParameterList", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    parameters = ApplicationParametersConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationCapacity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationCapacity = ApplicationCapacityDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("ManagedApplicationIdentity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    managedApplicationIdentity = ManagedApplicationIdentityDescriptionConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationDescription(
                name: name,
                typeName: typeName,
                typeVersion: typeVersion,
                parameters: parameters,
                applicationCapacity: applicationCapacity,
                managedApplicationIdentity: managedApplicationIdentity);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Name, "Name", ApplicationNameConverter.Serialize);
            writer.WriteProperty(obj.TypeName, "TypeName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.TypeVersion, "TypeVersion", JsonWriterExtensions.WriteStringValue);
            if (obj.Parameters != null)
            {
                writer.WriteProperty(obj.Parameters, "ParameterList", ApplicationParametersConverter.Serialize);
            }

            if (obj.ApplicationCapacity != null)
            {
                writer.WriteProperty(obj.ApplicationCapacity, "ApplicationCapacity", ApplicationCapacityDescriptionConverter.Serialize);
            }

            if (obj.ManagedApplicationIdentity != null)
            {
                writer.WriteProperty(obj.ManagedApplicationIdentity, "ManagedApplicationIdentity", ManagedApplicationIdentityDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
