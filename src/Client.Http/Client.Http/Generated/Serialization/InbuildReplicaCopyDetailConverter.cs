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
    /// Converter for <see cref="InbuildReplicaCopyDetail" />.
    /// </summary>
    internal class InbuildReplicaCopyDetailConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static InbuildReplicaCopyDetail Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static InbuildReplicaCopyDetail GetFromJsonProperties(JsonReader reader)
        {
            InbuildReplicaCopyDetail obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is Kind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("KeyValueStore", StringComparison.OrdinalIgnoreCase))
            {
                obj = KeyValueStoreReplicaCopyDetailConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown ReplicaKind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, InbuildReplicaCopyDetail obj)
        {
            var kind = obj.ReplicaKind;
            if (kind.Equals(ReplicaKind.KeyValueStore))
            {
                KeyValueStoreReplicaCopyDetailConverter.Serialize(writer, (KeyValueStoreReplicaCopyDetail)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown ReplicaKind.");
            }
        }
    }
}
