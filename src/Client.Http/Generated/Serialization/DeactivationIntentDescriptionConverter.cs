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
    /// Converter for <see cref="DeactivationIntentDescription" />.
    /// </summary>
    internal class DeactivationIntentDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DeactivationIntentDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DeactivationIntentDescription GetFromJsonProperties(JsonReader reader)
        {
            var deactivationIntent = default(DeactivationIntent?);
            var deactivationDescription = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("DeactivationIntent", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deactivationIntent = DeactivationIntentConverter.Deserialize(reader);
                }
                else if (string.Compare("DeactivationDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deactivationDescription = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DeactivationIntentDescription(
                deactivationIntent: deactivationIntent,
                deactivationDescription: deactivationDescription);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DeactivationIntentDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.DeactivationIntent, "DeactivationIntent", DeactivationIntentConverter.Serialize);
            if (obj.DeactivationDescription != null)
            {
                writer.WriteProperty(obj.DeactivationDescription, "DeactivationDescription", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
