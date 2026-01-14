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
    /// Converter for <see cref="GatewayProperties" />.
    /// </summary>
    internal class GatewayPropertiesConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static GatewayProperties Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static GatewayProperties GetFromJsonProperties(JsonReader reader)
        {
            var description = default(string);
            var sourceNetwork = default(NetworkRef);
            var destinationNetwork = default(NetworkRef);
            var tcp = default(IEnumerable<TcpConfig>);
            var http = default(IEnumerable<HttpConfig>);
            var status = default(ResourceStatus?);
            var statusDetails = default(string);
            var ipAddress = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("description", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    description = reader.ReadValueAsString();
                }
                else if (string.Compare("sourceNetwork", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sourceNetwork = NetworkRefConverter.Deserialize(reader);
                }
                else if (string.Compare("destinationNetwork", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    destinationNetwork = NetworkRefConverter.Deserialize(reader);
                }
                else if (string.Compare("tcp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tcp = reader.ReadList(TcpConfigConverter.Deserialize);
                }
                else if (string.Compare("http", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    http = reader.ReadList(HttpConfigConverter.Deserialize);
                }
                else if (string.Compare("status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = ResourceStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("statusDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    statusDetails = reader.ReadValueAsString();
                }
                else if (string.Compare("ipAddress", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ipAddress = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            var gatewayProperties = new GatewayProperties(
                description: description,
                sourceNetwork: sourceNetwork,
                destinationNetwork: destinationNetwork,
                tcp: tcp,
                http: http);

            gatewayProperties.Status = status;
            gatewayProperties.StatusDetails = statusDetails;
            gatewayProperties.IpAddress = ipAddress;
            return gatewayProperties;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, GatewayProperties obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.SourceNetwork, "sourceNetwork", NetworkRefConverter.Serialize);
            writer.WriteProperty(obj.DestinationNetwork, "destinationNetwork", NetworkRefConverter.Serialize);
            writer.WriteProperty(obj.Status, "status", ResourceStatusConverter.Serialize);
            if (obj.Description != null)
            {
                writer.WriteProperty(obj.Description, "description", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Tcp != null)
            {
                writer.WriteEnumerableProperty(obj.Tcp, "tcp", TcpConfigConverter.Serialize);
            }

            if (obj.Http != null)
            {
                writer.WriteEnumerableProperty(obj.Http, "http", HttpConfigConverter.Serialize);
            }

            if (obj.StatusDetails != null)
            {
                writer.WriteProperty(obj.StatusDetails, "statusDetails", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.IpAddress != null)
            {
                writer.WriteProperty(obj.IpAddress, "ipAddress", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
