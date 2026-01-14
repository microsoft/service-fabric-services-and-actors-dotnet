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
    /// Converter for <see cref="ResolvedServicePartition" />.
    /// </summary>
    internal class ResolvedServicePartitionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ResolvedServicePartition Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ResolvedServicePartition GetFromJsonProperties(JsonReader reader)
        {
            var name = default(ServiceName);
            var partitionInformation = default(PartitionInformation);
            var endpoints = default(IEnumerable<ResolvedServiceEndpoint>);
            var version = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = ServiceNameConverter.Deserialize(reader);
                }
                else if (string.Compare("PartitionInformation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionInformation = PartitionInformationConverter.Deserialize(reader);
                }
                else if (string.Compare("Endpoints", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    endpoints = reader.ReadList(ResolvedServiceEndpointConverter.Deserialize);
                }
                else if (string.Compare("Version", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    version = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ResolvedServicePartition(
                name: name,
                partitionInformation: partitionInformation,
                endpoints: endpoints,
                version: version);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ResolvedServicePartition obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Name, "Name", ServiceNameConverter.Serialize);
            writer.WriteProperty(obj.PartitionInformation, "PartitionInformation", PartitionInformationConverter.Serialize);
            writer.WriteEnumerableProperty(obj.Endpoints, "Endpoints", ResolvedServiceEndpointConverter.Serialize);
            writer.WriteProperty(obj.Version, "Version", JsonWriterExtensions.WriteStringValue);
            writer.WriteEndObject();
        }
    }
}
