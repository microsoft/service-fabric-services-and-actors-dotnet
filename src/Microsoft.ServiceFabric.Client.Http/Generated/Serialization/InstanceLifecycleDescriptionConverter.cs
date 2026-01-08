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
    /// Converter for <see cref="InstanceLifecycleDescription" />.
    /// </summary>
    internal class InstanceLifecycleDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static InstanceLifecycleDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static InstanceLifecycleDescription GetFromJsonProperties(JsonReader reader)
        {
            var restoreReplicaLocationAfterUpgrade = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("RestoreReplicaLocationAfterUpgrade", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restoreReplicaLocationAfterUpgrade = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new InstanceLifecycleDescription(
                restoreReplicaLocationAfterUpgrade: restoreReplicaLocationAfterUpgrade);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, InstanceLifecycleDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.RestoreReplicaLocationAfterUpgrade != null)
            {
                writer.WriteProperty(obj.RestoreReplicaLocationAfterUpgrade, "RestoreReplicaLocationAfterUpgrade", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
