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
    /// Converter for <see cref="IdentityDescription" />.
    /// </summary>
    internal class IdentityDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static IdentityDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static IdentityDescription GetFromJsonProperties(JsonReader reader)
        {
            var tokenServiceEndpoint = default(string);
            var type = default(string);
            var tenantId = default(string);
            var principalId = default(string);
            var userAssignedIdentities = default(IReadOnlyDictionary<string, IdentityItemDescription>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("tokenServiceEndpoint", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tokenServiceEndpoint = reader.ReadValueAsString();
                }
                else if (string.Compare("type", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    type = reader.ReadValueAsString();
                }
                else if (string.Compare("tenantId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tenantId = reader.ReadValueAsString();
                }
                else if (string.Compare("principalId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    principalId = reader.ReadValueAsString();
                }
                else if (string.Compare("userAssignedIdentities", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    userAssignedIdentities = reader.ReadDictionary(IdentityItemDescriptionConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new IdentityDescription(
                tokenServiceEndpoint: tokenServiceEndpoint,
                type: type,
                tenantId: tenantId,
                principalId: principalId,
                userAssignedIdentities: userAssignedIdentities);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, IdentityDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Type, "type", JsonWriterExtensions.WriteStringValue);
            if (obj.TokenServiceEndpoint != null)
            {
                writer.WriteProperty(obj.TokenServiceEndpoint, "tokenServiceEndpoint", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.TenantId != null)
            {
                writer.WriteProperty(obj.TenantId, "tenantId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PrincipalId != null)
            {
                writer.WriteProperty(obj.PrincipalId, "principalId", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.UserAssignedIdentities != null)
            {
                writer.WriteDictionaryProperty(obj.UserAssignedIdentities, "userAssignedIdentities", IdentityItemDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
