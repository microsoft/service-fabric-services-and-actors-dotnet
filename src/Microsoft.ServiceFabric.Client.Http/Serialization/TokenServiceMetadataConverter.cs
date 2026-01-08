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
    /// Converter for <see cref="TokenServiceMetadata" />.
    /// </summary>
    internal class TokenServiceMetadataConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static TokenServiceMetadata Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static TokenServiceMetadata GetFromJsonProperties(JsonReader reader)
        {
            var metadata = default(string);
            var serviceName = default(string);
            var serviceDnsName = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Metadata", propName, StringComparison.Ordinal) == 0)
                {
                    metadata = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceName", propName, StringComparison.Ordinal) == 0)
                {
                    serviceName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceDnsName", propName, StringComparison.Ordinal) == 0)
                {
                    serviceDnsName = reader.ReadValueAsString();
                }                
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new TokenServiceMetadata(
                metadata: metadata,
                serviceName: serviceName,
                serviceDnsName: serviceDnsName);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, TokenServiceMetadata obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Metadata != null)
            {
                writer.WriteProperty(obj.Metadata, "Metadata", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceName != null)
            {
                writer.WriteProperty(obj.ServiceName, "ServiceName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceDnsName != null)
            {
                writer.WriteProperty(obj.ServiceDnsName, "ServiceDnsName", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
