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
    /// Converter for <see cref="FolderSizeInfo" />.
    /// </summary>
    internal class FolderSizeInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static FolderSizeInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static FolderSizeInfo GetFromJsonProperties(JsonReader reader)
        {
            var storeRelativePath = default(string);
            var folderSize = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("StoreRelativePath", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    storeRelativePath = reader.ReadValueAsString();
                }
                else if (string.Compare("FolderSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    folderSize = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new FolderSizeInfo(
                storeRelativePath: storeRelativePath,
                folderSize: folderSize);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, FolderSizeInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.StoreRelativePath != null)
            {
                writer.WriteProperty(obj.StoreRelativePath, "StoreRelativePath", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FolderSize != null)
            {
                writer.WriteProperty(obj.FolderSize, "FolderSize", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
