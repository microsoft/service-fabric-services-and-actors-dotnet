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
    /// Converter for <see cref="AzureInternalMonitoringPipelineSinkDescription" />.
    /// </summary>
    internal class AzureInternalMonitoringPipelineSinkDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AzureInternalMonitoringPipelineSinkDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AzureInternalMonitoringPipelineSinkDescription GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var description = default(string);
            var accountName = default(string);
            var namespaceProperty = default(string);
            var maConfigUrl = default(string);
            var fluentdConfigUrl = default(string);
            var autoKeyConfigUrl = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("description", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    description = reader.ReadValueAsString();
                }
                else if (string.Compare("accountName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    accountName = reader.ReadValueAsString();
                }
                else if (string.Compare("namespace", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    namespaceProperty = reader.ReadValueAsString();
                }
                else if (string.Compare("maConfigUrl", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maConfigUrl = reader.ReadValueAsString();
                }
                else if (string.Compare("fluentdConfigUrl", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    fluentdConfigUrl = reader.ReadValueAsString();
                }
                else if (string.Compare("autoKeyConfigUrl", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    autoKeyConfigUrl = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new AzureInternalMonitoringPipelineSinkDescription(
                name: name,
                description: description,
                accountName: accountName,
                namespaceProperty: namespaceProperty,
                maConfigUrl: maConfigUrl,
                fluentdConfigUrl: fluentdConfigUrl,
                autoKeyConfigUrl: autoKeyConfigUrl);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AzureInternalMonitoringPipelineSinkDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "kind", DiagnosticsSinkKindConverter.Serialize);
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Description != null)
            {
                writer.WriteProperty(obj.Description, "description", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.AccountName != null)
            {
                writer.WriteProperty(obj.AccountName, "accountName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NamespaceProperty != null)
            {
                writer.WriteProperty(obj.NamespaceProperty, "namespace", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.MaConfigUrl != null)
            {
                writer.WriteProperty(obj.MaConfigUrl, "maConfigUrl", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FluentdConfigUrl != null)
            {
                writer.WriteProperty(obj.FluentdConfigUrl, "fluentdConfigUrl", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.AutoKeyConfigUrl != null)
            {
                writer.WriteProperty(obj.AutoKeyConfigUrl, "autoKeyConfigUrl", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
