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
    /// Converter for <see cref="HealthStateCount" />.
    /// </summary>
    internal class HealthStateCountConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static HealthStateCount Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static HealthStateCount GetFromJsonProperties(JsonReader reader)
        {
            var okCount = default(long?);
            var warningCount = default(long?);
            var errorCount = default(long?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("OkCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    okCount = reader.ReadValueAsLong();
                }
                else if (string.Compare("WarningCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    warningCount = reader.ReadValueAsLong();
                }
                else if (string.Compare("ErrorCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    errorCount = reader.ReadValueAsLong();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new HealthStateCount(
                okCount: okCount,
                warningCount: warningCount,
                errorCount: errorCount);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, HealthStateCount obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.OkCount != null)
            {
                writer.WriteProperty(obj.OkCount, "OkCount", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.WarningCount != null)
            {
                writer.WriteProperty(obj.WarningCount, "WarningCount", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ErrorCount != null)
            {
                writer.WriteProperty(obj.ErrorCount, "ErrorCount", JsonWriterExtensions.WriteLongValue);
            }

            writer.WriteEndObject();
        }
    }
}
