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
    /// Converter for <see cref="PartitionHealthStateFilter" />.
    /// </summary>
    internal class PartitionHealthStateFilterConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionHealthStateFilter Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionHealthStateFilter GetFromJsonProperties(JsonReader reader)
        {
            var partitionIdFilter = default(Guid?);
            var healthStateFilter = default(int?);
            var replicaFilters = default(IEnumerable<ReplicaHealthStateFilter>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PartitionIdFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    partitionIdFilter = reader.ReadValueAsGuid();
                }
                else if (string.Compare("HealthStateFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthStateFilter = reader.ReadValueAsInt();
                }
                else if (string.Compare("ReplicaFilters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaFilters = reader.ReadList(ReplicaHealthStateFilterConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PartitionHealthStateFilter(
                partitionIdFilter: partitionIdFilter,
                healthStateFilter: healthStateFilter,
                replicaFilters: replicaFilters);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionHealthStateFilter obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.PartitionIdFilter != null)
            {
                writer.WriteProperty(obj.PartitionIdFilter, "PartitionIdFilter", JsonWriterExtensions.WriteGuidValue);
            }

            if (obj.HealthStateFilter != null)
            {
                writer.WriteProperty(obj.HealthStateFilter, "HealthStateFilter", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.ReplicaFilters != null)
            {
                writer.WriteEnumerableProperty(obj.ReplicaFilters, "ReplicaFilters", ReplicaHealthStateFilterConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
