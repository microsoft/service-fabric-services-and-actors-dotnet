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
    /// Converter for <see cref="ApplicationTypeInfo" />.
    /// </summary>
    internal class ApplicationTypeInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationTypeInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationTypeInfo GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var version = default(string);
            var defaultParameterList = default(IReadOnlyDictionary<string, string>);
            var status = default(ApplicationTypeStatus?);
            var statusDetails = default(string);
            var applicationTypeDefinitionKind = default(ApplicationTypeDefinitionKind?);
            var applicationTypeMetadata = default(ApplicationTypeMetadata);
            var managedKeyVaultReferenceParameters = default(IEnumerable<ManagedKeyVaultReferenceParameter>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("Version", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    version = reader.ReadValueAsString();
                }
                else if (string.Compare("DefaultParameterList", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    defaultParameterList = ApplicationParametersConverter.Deserialize(reader);
                }
                else if (string.Compare("Status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = ApplicationTypeStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("StatusDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    statusDetails = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationTypeDefinitionKind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeDefinitionKind = ApplicationTypeDefinitionKindConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationTypeMetadata", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeMetadata = ApplicationTypeMetadataConverter.Deserialize(reader);
                }
                else if (string.Compare("ManagedKeyVaultReferenceParameters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    managedKeyVaultReferenceParameters = reader.ReadList(ManagedKeyVaultReferenceParameterConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationTypeInfo(
                name: name,
                version: version,
                defaultParameterList: defaultParameterList,
                status: status,
                statusDetails: statusDetails,
                applicationTypeDefinitionKind: applicationTypeDefinitionKind,
                applicationTypeMetadata: applicationTypeMetadata,
                managedKeyVaultReferenceParameters: managedKeyVaultReferenceParameters);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationTypeInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Status, "Status", ApplicationTypeStatusConverter.Serialize);
            writer.WriteProperty(obj.ApplicationTypeDefinitionKind, "ApplicationTypeDefinitionKind", ApplicationTypeDefinitionKindConverter.Serialize);
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Version != null)
            {
                writer.WriteProperty(obj.Version, "Version", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.DefaultParameterList != null)
            {
                writer.WriteProperty(obj.DefaultParameterList, "DefaultParameterList", ApplicationParametersConverter.Serialize);
            }

            if (obj.StatusDetails != null)
            {
                writer.WriteProperty(obj.StatusDetails, "StatusDetails", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ApplicationTypeMetadata != null)
            {
                writer.WriteProperty(obj.ApplicationTypeMetadata, "ApplicationTypeMetadata", ApplicationTypeMetadataConverter.Serialize);
            }

            if (obj.ManagedKeyVaultReferenceParameters != null)
            {
                writer.WriteEnumerableProperty(obj.ManagedKeyVaultReferenceParameters, "ManagedKeyVaultReferenceParameters", ManagedKeyVaultReferenceParameterConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
