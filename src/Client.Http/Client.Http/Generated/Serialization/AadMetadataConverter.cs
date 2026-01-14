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
    /// Converter for <see cref="AadMetadata" />.
    /// </summary>
    internal class AadMetadataConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AadMetadata Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AadMetadata GetFromJsonProperties(JsonReader reader)
        {
            var authority = default(string);
            var client = default(string);
            var cluster = default(string);
            var login = default(string);
            var redirect = default(string);
            var tenant = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("authority", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    authority = reader.ReadValueAsString();
                }
                else if (string.Compare("client", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    client = reader.ReadValueAsString();
                }
                else if (string.Compare("cluster", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cluster = reader.ReadValueAsString();
                }
                else if (string.Compare("login", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    login = reader.ReadValueAsString();
                }
                else if (string.Compare("redirect", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    redirect = reader.ReadValueAsString();
                }
                else if (string.Compare("tenant", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tenant = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new AadMetadata(
                authority: authority,
                client: client,
                cluster: cluster,
                login: login,
                redirect: redirect,
                tenant: tenant);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AadMetadata obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Authority != null)
            {
                writer.WriteProperty(obj.Authority, "authority", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Client != null)
            {
                writer.WriteProperty(obj.Client, "client", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Cluster != null)
            {
                writer.WriteProperty(obj.Cluster, "cluster", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Login != null)
            {
                writer.WriteProperty(obj.Login, "login", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Redirect != null)
            {
                writer.WriteProperty(obj.Redirect, "redirect", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Tenant != null)
            {
                writer.WriteProperty(obj.Tenant, "tenant", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
