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
    /// Converter for <see cref="ApplicationTypeMetadata" />.
    /// </summary>
    internal class ApplicationTypeMetadataConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationTypeMetadata Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationTypeMetadata GetFromJsonProperties(JsonReader reader)
        {
            var applicationTypeProvisionTimestamp = default(string);
            var armMetadata = default(ArmMetadata);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ApplicationTypeProvisionTimestamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeProvisionTimestamp = reader.ReadValueAsString();
                }
                else if (string.Compare("ArmMetadata", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    armMetadata = ArmMetadataConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationTypeMetadata(
                applicationTypeProvisionTimestamp: applicationTypeProvisionTimestamp,
                armMetadata: armMetadata);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationTypeMetadata obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ApplicationTypeProvisionTimestamp != null)
            {
                writer.WriteProperty(obj.ApplicationTypeProvisionTimestamp, "ApplicationTypeProvisionTimestamp", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ArmMetadata != null)
            {
                writer.WriteProperty(obj.ArmMetadata, "ArmMetadata", ArmMetadataConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
