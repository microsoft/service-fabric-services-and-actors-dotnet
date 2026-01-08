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
    /// Converter for <see cref="SecretResourceProperties" />.
    /// </summary>
    internal class SecretResourcePropertiesConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static SecretResourceProperties Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static SecretResourceProperties GetFromJsonProperties(JsonReader reader)
        {
            var kind = default(SecretKind?);
            var description = default(string);
            var status = default(ResourceStatus?);
            var statusDetails = default(string);
            var contentType = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("kind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    kind = SecretKindConverter.Deserialize(reader);
                }
                else if (string.Compare("description", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    description = reader.ReadValueAsString();
                }
                else if (string.Compare("status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = ResourceStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("statusDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    statusDetails = reader.ReadValueAsString();
                }
                else if (string.Compare("contentType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    contentType = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            var secretResourceProperties = new SecretResourceProperties(
                kind: kind,
                description: description,
                contentType: contentType);

            secretResourceProperties.Status = status;
            secretResourceProperties.StatusDetails = statusDetails;
            return secretResourceProperties;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, SecretResourceProperties obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "kind", SecretKindConverter.Serialize);
            writer.WriteProperty(obj.Status, "status", ResourceStatusConverter.Serialize);
            if (obj.Description != null)
            {
                writer.WriteProperty(obj.Description, "description", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.StatusDetails != null)
            {
                writer.WriteProperty(obj.StatusDetails, "statusDetails", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ContentType != null)
            {
                writer.WriteProperty(obj.ContentType, "contentType", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
