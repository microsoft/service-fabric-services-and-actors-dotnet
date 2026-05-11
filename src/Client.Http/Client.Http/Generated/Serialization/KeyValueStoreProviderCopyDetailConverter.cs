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
    /// Converter for <see cref="KeyValueStoreProviderCopyDetail" />.
    /// </summary>
    internal class KeyValueStoreProviderCopyDetailConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static KeyValueStoreProviderCopyDetail Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static KeyValueStoreProviderCopyDetail GetFromJsonProperties(JsonReader reader)
        {
            KeyValueStoreProviderCopyDetail obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("ProviderKind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is ProviderKind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("ESE", StringComparison.OrdinalIgnoreCase))
            {
                obj = KeyValueStoreESEReplicaCopyDetailConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown ProviderKind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, KeyValueStoreProviderCopyDetail obj)
        {
            var kind = obj.ProviderKind;
            if (kind.Equals(KeyValueStoreProviderKind.ESE))
            {
                KeyValueStoreESEReplicaCopyDetailConverter.Serialize(writer, (KeyValueStoreESEReplicaCopyDetail)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown ProviderKind.");
            }
        }
    }
}
