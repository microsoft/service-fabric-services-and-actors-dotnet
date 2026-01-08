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
    /// Converter for <see cref="ClusterLoadInfo" />.
    /// </summary>
    internal class ClusterLoadInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterLoadInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterLoadInfo GetFromJsonProperties(JsonReader reader)
        {
            var lastBalancingStartTimeUtc = default(DateTime?);
            var lastBalancingEndTimeUtc = default(DateTime?);
            var loadMetricInformation = default(IEnumerable<LoadMetricInformation>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("LastBalancingStartTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastBalancingStartTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LastBalancingEndTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    lastBalancingEndTimeUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("LoadMetricInformation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    loadMetricInformation = reader.ReadList(LoadMetricInformationConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ClusterLoadInfo(
                lastBalancingStartTimeUtc: lastBalancingStartTimeUtc,
                lastBalancingEndTimeUtc: lastBalancingEndTimeUtc,
                loadMetricInformation: loadMetricInformation);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ClusterLoadInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.LastBalancingStartTimeUtc != null)
            {
                writer.WriteProperty(obj.LastBalancingStartTimeUtc, "LastBalancingStartTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LastBalancingEndTimeUtc != null)
            {
                writer.WriteProperty(obj.LastBalancingEndTimeUtc, "LastBalancingEndTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.LoadMetricInformation != null)
            {
                writer.WriteEnumerableProperty(obj.LoadMetricInformation, "LoadMetricInformation", LoadMetricInformationConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
