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
    /// Converter for <see cref="ApplicationInfo" />.
    /// </summary>
    internal class ApplicationInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationInfo GetFromJsonProperties(JsonReader reader)
        {
            var id = default(string);
            var name = default(ApplicationName);
            var typeName = default(string);
            var typeVersion = default(string);
            var status = default(ApplicationStatus?);
            var parameters = default(IReadOnlyDictionary<string, string>);
            var healthState = default(HealthState?);
            var applicationDefinitionKind = default(ApplicationDefinitionKind?);
            var managedApplicationIdentity = default(ManagedApplicationIdentityDescription);
            var applicationMetadata = default(ApplicationMetadata);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Id", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    id = reader.ReadValueAsString();
                }
                else if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
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
                else if (string.Compare("Status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = ApplicationStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("Parameters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    parameters = ApplicationParametersConverter.Deserialize(reader);
                }
                else if (string.Compare("HealthState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthState = HealthStateConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationDefinitionKind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationDefinitionKind = ApplicationDefinitionKindConverter.Deserialize(reader);
                }
                else if (string.Compare("ManagedApplicationIdentity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    managedApplicationIdentity = ManagedApplicationIdentityDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationMetadata", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationMetadata = ApplicationMetadataConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationInfo(
                id: id,
                name: name,
                typeName: typeName,
                typeVersion: typeVersion,
                status: status,
                parameters: parameters,
                healthState: healthState,
                applicationDefinitionKind: applicationDefinitionKind,
                managedApplicationIdentity: managedApplicationIdentity,
                applicationMetadata: applicationMetadata);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Status, "Status", ApplicationStatusConverter.Serialize);
            writer.WriteProperty(obj.HealthState, "HealthState", HealthStateConverter.Serialize);
            writer.WriteProperty(obj.ApplicationDefinitionKind, "ApplicationDefinitionKind", ApplicationDefinitionKindConverter.Serialize);
            if (obj.Id != null)
            {
                writer.WriteProperty(obj.Id, "Id", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", ApplicationNameConverter.Serialize);
            }

            if (obj.TypeName != null)
            {
                writer.WriteProperty(obj.TypeName, "TypeName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.TypeVersion != null)
            {
                writer.WriteProperty(obj.TypeVersion, "TypeVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Parameters != null)
            {
                writer.WriteProperty(obj.Parameters, "Parameters", ApplicationParametersConverter.Serialize);
            }

            if (obj.ManagedApplicationIdentity != null)
            {
                writer.WriteProperty(obj.ManagedApplicationIdentity, "ManagedApplicationIdentity", ManagedApplicationIdentityDescriptionConverter.Serialize);
            }

            if (obj.ApplicationMetadata != null)
            {
                writer.WriteProperty(obj.ApplicationMetadata, "ApplicationMetadata", ApplicationMetadataConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
