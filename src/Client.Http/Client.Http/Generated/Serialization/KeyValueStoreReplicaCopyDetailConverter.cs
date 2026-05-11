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
    /// Converter for <see cref="KeyValueStoreReplicaCopyDetail" />.
    /// </summary>
    internal class KeyValueStoreReplicaCopyDetailConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static KeyValueStoreReplicaCopyDetail Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static KeyValueStoreReplicaCopyDetail GetFromJsonProperties(JsonReader reader)
        {
            var providerCopyDetail = default(KeyValueStoreProviderCopyDetail);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ProviderCopyDetail", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    providerCopyDetail = KeyValueStoreProviderCopyDetailConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new KeyValueStoreReplicaCopyDetail(
                providerCopyDetail: providerCopyDetail);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, KeyValueStoreReplicaCopyDetail obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ReplicaKind, "ReplicaKind", ReplicaKindConverter.Serialize);
            if (obj.ProviderCopyDetail != null)
            {
                writer.WriteProperty(obj.ProviderCopyDetail, "ProviderCopyDetail", KeyValueStoreProviderCopyDetailConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
