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
    /// Converter for <see cref="ReplicaHealthStateFilter" />.
    /// </summary>
    internal class ReplicaHealthStateFilterConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicaHealthStateFilter Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ReplicaHealthStateFilter GetFromJsonProperties(JsonReader reader)
        {
            var replicaOrInstanceIdFilter = default(string);
            var healthStateFilter = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ReplicaOrInstanceIdFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaOrInstanceIdFilter = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthStateFilter", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthStateFilter = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ReplicaHealthStateFilter(
                replicaOrInstanceIdFilter: replicaOrInstanceIdFilter,
                healthStateFilter: healthStateFilter);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ReplicaHealthStateFilter obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ReplicaOrInstanceIdFilter != null)
            {
                writer.WriteProperty(obj.ReplicaOrInstanceIdFilter, "ReplicaOrInstanceIdFilter", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HealthStateFilter != null)
            {
                writer.WriteProperty(obj.HealthStateFilter, "HealthStateFilter", JsonWriterExtensions.WriteIntValue);
            }

            writer.WriteEndObject();
        }
    }
}
