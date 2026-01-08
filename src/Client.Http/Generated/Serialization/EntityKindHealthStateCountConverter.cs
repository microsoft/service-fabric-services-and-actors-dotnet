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
    /// Converter for <see cref="EntityKindHealthStateCount" />.
    /// </summary>
    internal class EntityKindHealthStateCountConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static EntityKindHealthStateCount Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static EntityKindHealthStateCount GetFromJsonProperties(JsonReader reader)
        {
            var entityKind = default(EntityKind?);
            var healthStateCount = default(HealthStateCount);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("EntityKind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    entityKind = EntityKindConverter.Deserialize(reader);
                }
                else if (string.Compare("HealthStateCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthStateCount = HealthStateCountConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new EntityKindHealthStateCount(
                entityKind: entityKind,
                healthStateCount: healthStateCount);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, EntityKindHealthStateCount obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.EntityKind, "EntityKind", EntityKindConverter.Serialize);
            if (obj.HealthStateCount != null)
            {
                writer.WriteProperty(obj.HealthStateCount, "HealthStateCount", HealthStateCountConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
