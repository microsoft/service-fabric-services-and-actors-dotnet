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
    /// Converter for <see cref="ConfigParameterOverride" />.
    /// </summary>
    internal class ConfigParameterOverrideConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ConfigParameterOverride Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ConfigParameterOverride GetFromJsonProperties(JsonReader reader)
        {
            var sectionName = default(string);
            var parameterName = default(string);
            var parameterValue = default(string);
            var timeout = default(TimeSpan?);
            var persistAcrossUpgrade = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("SectionName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sectionName = reader.ReadValueAsString();
                }
                else if (string.Compare("ParameterName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    parameterName = reader.ReadValueAsString();
                }
                else if (string.Compare("ParameterValue", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    parameterValue = reader.ReadValueAsString();
                }
                else if (string.Compare("Timeout", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeout = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("PersistAcrossUpgrade", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    persistAcrossUpgrade = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ConfigParameterOverride(
                sectionName: sectionName,
                parameterName: parameterName,
                parameterValue: parameterValue,
                timeout: timeout,
                persistAcrossUpgrade: persistAcrossUpgrade);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ConfigParameterOverride obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.SectionName, "SectionName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ParameterName, "ParameterName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ParameterValue, "ParameterValue", JsonWriterExtensions.WriteStringValue);
            if (obj.Timeout != null)
            {
                writer.WriteProperty(obj.Timeout, "Timeout", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.PersistAcrossUpgrade != null)
            {
                writer.WriteProperty(obj.PersistAcrossUpgrade, "PersistAcrossUpgrade", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
