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
    /// Converter for <see cref="UnprovisionFabricDescription" />.
    /// </summary>
    internal class UnprovisionFabricDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static UnprovisionFabricDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static UnprovisionFabricDescription GetFromJsonProperties(JsonReader reader)
        {
            var codeVersion = default(string);
            var configVersion = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("CodeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("ConfigVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    configVersion = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new UnprovisionFabricDescription(
                codeVersion: codeVersion,
                configVersion: configVersion);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, UnprovisionFabricDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.CodeVersion != null)
            {
                writer.WriteProperty(obj.CodeVersion, "CodeVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ConfigVersion != null)
            {
                writer.WriteProperty(obj.ConfigVersion, "ConfigVersion", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
