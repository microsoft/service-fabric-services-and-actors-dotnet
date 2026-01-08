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
    /// Converter for <see cref="VolumeProviderParametersAzureFile" />.
    /// </summary>
    internal class VolumeProviderParametersAzureFileConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static VolumeProviderParametersAzureFile Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static VolumeProviderParametersAzureFile GetFromJsonProperties(JsonReader reader)
        {
            var accountName = default(string);
            var accountKey = default(string);
            var shareName = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("accountName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    accountName = reader.ReadValueAsString();
                }
                else if (string.Compare("accountKey", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    accountKey = reader.ReadValueAsString();
                }
                else if (string.Compare("shareName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    shareName = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new VolumeProviderParametersAzureFile(
                accountName: accountName,
                accountKey: accountKey,
                shareName: shareName);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, VolumeProviderParametersAzureFile obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.AccountName, "accountName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ShareName, "shareName", JsonWriterExtensions.WriteStringValue);
            if (obj.AccountKey != null)
            {
                writer.WriteProperty(obj.AccountKey, "accountKey", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
