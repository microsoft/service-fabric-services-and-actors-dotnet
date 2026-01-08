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
    /// Converter for <see cref="GatewayDestination" />.
    /// </summary>
    internal class GatewayDestinationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static GatewayDestination Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static GatewayDestination GetFromJsonProperties(JsonReader reader)
        {
            var applicationName = default(string);
            var serviceName = default(string);
            var endpointName = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("applicationName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationName = reader.ReadValueAsString();
                }
                else if (string.Compare("serviceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = reader.ReadValueAsString();
                }
                else if (string.Compare("endpointName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    endpointName = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new GatewayDestination(
                applicationName: applicationName,
                serviceName: serviceName,
                endpointName: endpointName);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, GatewayDestination obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ApplicationName, "applicationName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ServiceName, "serviceName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.EndpointName, "endpointName", JsonWriterExtensions.WriteStringValue);
            writer.WriteEndObject();
        }
    }
}
