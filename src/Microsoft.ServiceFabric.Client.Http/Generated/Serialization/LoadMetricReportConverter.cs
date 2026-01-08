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
    /// Converter for <see cref="LoadMetricReport" />.
    /// </summary>
    internal class LoadMetricReportConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static LoadMetricReport Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static LoadMetricReport GetFromJsonProperties(JsonReader reader)
        {
            var lastReportedUtc = default(DateTime?);
            var name = default(string);
            var value = default(string);
            var currentValue = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("LastReportedUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastReportedUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("Value", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = reader.ReadValueAsString();
                }
                else if (string.Compare("CurrentValue", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentValue = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new LoadMetricReport(
                lastReportedUtc: lastReportedUtc,
                name: name,
                value: value,
                currentValue: currentValue);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, LoadMetricReport obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.LastReportedUtc != null)
            {
                writer.WriteProperty(obj.LastReportedUtc, "LastReportedUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Value != null)
            {
                writer.WriteProperty(obj.Value, "Value", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CurrentValue != null)
            {
                writer.WriteProperty(obj.CurrentValue, "CurrentValue", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
