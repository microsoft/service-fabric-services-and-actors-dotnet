// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    sealed class TokenServiceMetadataConverter
    {
        internal static TokenServiceMetadata Deserialize(JsonReader reader) =>
            reader.Deserialize(GetFromJsonProperties);

        internal static TokenServiceMetadata GetFromJsonProperties(JsonReader reader)
        {
            string metadata = default;
            string serviceName = default;
            string serviceDnsName = default;

            do
            {
                string propName = reader.ReadPropertyName();
                if (string.Compare("Metadata", propName, StringComparison.Ordinal) == 0)
                    metadata = reader.ReadValueAsString();
                else if (string.Compare("ServiceName", propName, StringComparison.Ordinal) == 0)
                    serviceName = reader.ReadValueAsString();
                else if (string.Compare("ServiceDnsName", propName, StringComparison.Ordinal) == 0)
                    serviceDnsName = reader.ReadValueAsString();
                else
                    reader.SkipPropertyValue();
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new TokenServiceMetadata(metadata, serviceName, serviceDnsName);
        }

        internal static void Serialize(JsonWriter writer, TokenServiceMetadata obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Metadata != null)
                writer.WriteProperty(obj.Metadata, "Metadata", JsonWriterExtensions.WriteStringValue);

            if (obj.ServiceName != null)
                writer.WriteProperty(obj.ServiceName, "ServiceName", JsonWriterExtensions.WriteStringValue);

            if (obj.ServiceDnsName != null)
                writer.WriteProperty(obj.ServiceDnsName, "ServiceDnsName", JsonWriterExtensions.WriteStringValue);

            writer.WriteEndObject();
        }
    }
}
