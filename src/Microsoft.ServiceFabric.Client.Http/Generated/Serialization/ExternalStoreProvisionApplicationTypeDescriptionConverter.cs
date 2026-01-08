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
    /// Converter for <see cref="ExternalStoreProvisionApplicationTypeDescription" />.
    /// </summary>
    internal class ExternalStoreProvisionApplicationTypeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ExternalStoreProvisionApplicationTypeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ExternalStoreProvisionApplicationTypeDescription GetFromJsonProperties(JsonReader reader)
        {
            var async = default(bool?);
            var applicationPackageDownloadUri = default(string);
            var applicationTypeName = default(string);
            var applicationTypeVersion = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Async", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    async = reader.ReadValueAsBool();
                }
                else if (string.Compare("ApplicationPackageDownloadUri", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationPackageDownloadUri = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationTypeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeName = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationTypeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeVersion = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ExternalStoreProvisionApplicationTypeDescription(
                async: async,
                applicationPackageDownloadUri: applicationPackageDownloadUri,
                applicationTypeName: applicationTypeName,
                applicationTypeVersion: applicationTypeVersion);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ExternalStoreProvisionApplicationTypeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ProvisionApplicationTypeKindConverter.Serialize);
            writer.WriteProperty(obj.ApplicationPackageDownloadUri, "ApplicationPackageDownloadUri", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ApplicationTypeName, "ApplicationTypeName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ApplicationTypeVersion, "ApplicationTypeVersion", JsonWriterExtensions.WriteStringValue);
            if (obj.Async != null)
            {
                writer.WriteProperty(obj.Async, "Async", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
