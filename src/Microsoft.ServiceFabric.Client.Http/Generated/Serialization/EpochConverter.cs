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
    /// Converter for <see cref="Epoch" />.
    /// </summary>
    internal class EpochConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static Epoch Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static Epoch GetFromJsonProperties(JsonReader reader)
        {
            var configurationVersion = default(string);
            var dataLossVersion = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ConfigurationVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    configurationVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("DataLossVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    dataLossVersion = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new Epoch(
                configurationVersion: configurationVersion,
                dataLossVersion: dataLossVersion);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, Epoch obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ConfigurationVersion != null)
            {
                writer.WriteProperty(obj.ConfigurationVersion, "ConfigurationVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.DataLossVersion != null)
            {
                writer.WriteProperty(obj.DataLossVersion, "DataLossVersion", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
