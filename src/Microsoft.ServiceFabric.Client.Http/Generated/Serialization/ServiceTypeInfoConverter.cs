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
    /// Converter for <see cref="ServiceTypeInfo" />.
    /// </summary>
    internal class ServiceTypeInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceTypeInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceTypeInfo GetFromJsonProperties(JsonReader reader)
        {
            var serviceTypeDescription = default(ServiceTypeDescription);
            var serviceManifestName = default(string);
            var serviceManifestVersion = default(string);
            var isServiceGroup = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ServiceTypeDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceTypeDescription = ServiceTypeDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("ServiceManifestName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceManifestName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceManifestVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceManifestVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("IsServiceGroup", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isServiceGroup = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceTypeInfo(
                serviceTypeDescription: serviceTypeDescription,
                serviceManifestName: serviceManifestName,
                serviceManifestVersion: serviceManifestVersion,
                isServiceGroup: isServiceGroup);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceTypeInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ServiceTypeDescription != null)
            {
                writer.WriteProperty(obj.ServiceTypeDescription, "ServiceTypeDescription", ServiceTypeDescriptionConverter.Serialize);
            }

            if (obj.ServiceManifestName != null)
            {
                writer.WriteProperty(obj.ServiceManifestName, "ServiceManifestName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceManifestVersion != null)
            {
                writer.WriteProperty(obj.ServiceManifestVersion, "ServiceManifestVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.IsServiceGroup != null)
            {
                writer.WriteProperty(obj.IsServiceGroup, "IsServiceGroup", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
