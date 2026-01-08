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
    /// Converter for <see cref="ServiceHealthStateFilter" />.
    /// </summary>
    internal class ServiceHealthStateFilterConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceHealthStateFilter Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceHealthStateFilter GetFromJsonProperties(JsonReader reader)
        {
            var serviceNameFilter = default(string);
            var healthStateFilter = default(int?);
            var partitionFilters = default(IEnumerable<PartitionHealthStateFilter>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ServiceNameFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceNameFilter = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthStateFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthStateFilter = reader.ReadValueAsInt();
                }
                else if (string.Compare("PartitionFilters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionFilters = reader.ReadList(PartitionHealthStateFilterConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceHealthStateFilter(
                serviceNameFilter: serviceNameFilter,
                healthStateFilter: healthStateFilter,
                partitionFilters: partitionFilters);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceHealthStateFilter obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ServiceNameFilter != null)
            {
                writer.WriteProperty(obj.ServiceNameFilter, "ServiceNameFilter", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HealthStateFilter != null)
            {
                writer.WriteProperty(obj.HealthStateFilter, "HealthStateFilter", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.PartitionFilters != null)
            {
                writer.WriteEnumerableProperty(obj.PartitionFilters, "PartitionFilters", PartitionHealthStateFilterConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
