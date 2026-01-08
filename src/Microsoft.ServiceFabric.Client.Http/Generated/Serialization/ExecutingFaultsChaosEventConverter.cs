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
    /// Converter for <see cref="ExecutingFaultsChaosEvent" />.
    /// </summary>
    internal class ExecutingFaultsChaosEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ExecutingFaultsChaosEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ExecutingFaultsChaosEvent GetFromJsonProperties(JsonReader reader)
        {
            var timeStampUtc = default(DateTime?);
            var faults = default(IEnumerable<string>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("TimeStampUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeStampUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("Faults", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    faults = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ExecutingFaultsChaosEvent(
                timeStampUtc: timeStampUtc,
                faults: faults);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ExecutingFaultsChaosEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ChaosEventKindConverter.Serialize);
            writer.WriteProperty(obj.TimeStampUtc, "TimeStampUtc", JsonWriterExtensions.WriteDateTimeValue);
            if (obj.Faults != null)
            {
                writer.WriteEnumerableProperty(obj.Faults, "Faults", (w, v) => writer.WriteStringValue(v));
            }

            writer.WriteEndObject();
        }
    }
}
