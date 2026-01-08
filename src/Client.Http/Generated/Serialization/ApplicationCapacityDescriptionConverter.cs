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
    /// Converter for <see cref="ApplicationCapacityDescription" />.
    /// </summary>
    internal class ApplicationCapacityDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationCapacityDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationCapacityDescription GetFromJsonProperties(JsonReader reader)
        {
            var minimumNodes = default(long?);
            var maximumNodes = default(long?);
            var applicationMetrics = default(IEnumerable<ApplicationMetricDescription>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("MinimumNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minimumNodes = reader.ReadValueAsLong();
                }
                else if (string.Compare("MaximumNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maximumNodes = reader.ReadValueAsLong();
                }
                else if (string.Compare("ApplicationMetrics", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationMetrics = reader.ReadList(ApplicationMetricDescriptionConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationCapacityDescription(
                minimumNodes: minimumNodes,
                maximumNodes: maximumNodes,
                applicationMetrics: applicationMetrics);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationCapacityDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.MinimumNodes != null)
            {
                writer.WriteProperty(obj.MinimumNodes, "MinimumNodes", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.MaximumNodes != null)
            {
                writer.WriteProperty(obj.MaximumNodes, "MaximumNodes", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ApplicationMetrics != null)
            {
                writer.WriteEnumerableProperty(obj.ApplicationMetrics, "ApplicationMetrics", ApplicationMetricDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
