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
    /// Converter for <see cref="LoadMetricReportInfo" />.
    /// </summary>
    internal class LoadMetricReportInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static LoadMetricReportInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static LoadMetricReportInfo GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var value = default(int?);
            var currentValue = default(string);
            var lastReportedUtc = default(DateTime?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("Value", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = reader.ReadValueAsInt();
                }
                else if (string.Compare("CurrentValue", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentValue = reader.ReadValueAsString();
                }
                else if (string.Compare("LastReportedUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastReportedUtc = reader.ReadValueAsDateTime();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new LoadMetricReportInfo(
                name: name,
                value: value,
                currentValue: currentValue,
                lastReportedUtc: lastReportedUtc);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, LoadMetricReportInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Value != null)
            {
                writer.WriteProperty(obj.Value, "Value", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.CurrentValue != null)
            {
                writer.WriteProperty(obj.CurrentValue, "CurrentValue", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.LastReportedUtc != null)
            {
                writer.WriteProperty(obj.LastReportedUtc, "LastReportedUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            writer.WriteEndObject();
        }
    }
}
