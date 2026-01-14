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
    /// Converter for <see cref="ServiceSensitivityDescription" />.
    /// </summary>
    internal class ServiceSensitivityDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceSensitivityDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceSensitivityDescription GetFromJsonProperties(JsonReader reader)
        {
            var primaryDefaultSensitivity = default(int?);
            var secondaryDefaultSensitivity = default(int?);
            var auxiliaryDefaultSensitivity = default(int?);
            var isMaximumSensitivity = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PrimaryDefaultSensitivity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryDefaultSensitivity = reader.ReadValueAsInt();
                }
                else if (string.Compare("SecondaryDefaultSensitivity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryDefaultSensitivity = reader.ReadValueAsInt();
                }
                else if (string.Compare("AuxiliaryDefaultSensitivity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    auxiliaryDefaultSensitivity = reader.ReadValueAsInt();
                }
                else if (string.Compare("IsMaximumSensitivity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isMaximumSensitivity = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceSensitivityDescription(
                primaryDefaultSensitivity: primaryDefaultSensitivity,
                secondaryDefaultSensitivity: secondaryDefaultSensitivity,
                auxiliaryDefaultSensitivity: auxiliaryDefaultSensitivity,
                isMaximumSensitivity: isMaximumSensitivity);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceSensitivityDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.PrimaryDefaultSensitivity != null)
            {
                writer.WriteProperty(obj.PrimaryDefaultSensitivity, "PrimaryDefaultSensitivity", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.SecondaryDefaultSensitivity != null)
            {
                writer.WriteProperty(obj.SecondaryDefaultSensitivity, "SecondaryDefaultSensitivity", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.AuxiliaryDefaultSensitivity != null)
            {
                writer.WriteProperty(obj.AuxiliaryDefaultSensitivity, "AuxiliaryDefaultSensitivity", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.IsMaximumSensitivity != null)
            {
                writer.WriteProperty(obj.IsMaximumSensitivity, "IsMaximumSensitivity", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
