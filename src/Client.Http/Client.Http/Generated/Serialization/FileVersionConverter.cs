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
    /// Converter for <see cref="FileVersion" />.
    /// </summary>
    internal class FileVersionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static FileVersion Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static FileVersion GetFromJsonProperties(JsonReader reader)
        {
            var versionNumber = default(string);
            var epochDataLossNumber = default(string);
            var epochConfigurationNumber = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("VersionNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    versionNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("EpochDataLossNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    epochDataLossNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("EpochConfigurationNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    epochConfigurationNumber = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new FileVersion(
                versionNumber: versionNumber,
                epochDataLossNumber: epochDataLossNumber,
                epochConfigurationNumber: epochConfigurationNumber);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, FileVersion obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.VersionNumber != null)
            {
                writer.WriteProperty(obj.VersionNumber, "VersionNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.EpochDataLossNumber != null)
            {
                writer.WriteProperty(obj.EpochDataLossNumber, "EpochDataLossNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.EpochConfigurationNumber != null)
            {
                writer.WriteProperty(obj.EpochConfigurationNumber, "EpochConfigurationNumber", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
