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
    /// Converter for <see cref="ImageStoreContent" />.
    /// </summary>
    internal class ImageStoreContentConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ImageStoreContent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ImageStoreContent GetFromJsonProperties(JsonReader reader)
        {
            var storeFiles = default(IEnumerable<FileInfo>);
            var storeFolders = default(IEnumerable<FolderInfo>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("StoreFiles", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    storeFiles = reader.ReadList(FileInfoConverter.Deserialize);
                }
                else if (string.Compare("StoreFolders", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    storeFolders = reader.ReadList(FolderInfoConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ImageStoreContent(
                storeFiles: storeFiles,
                storeFolders: storeFolders);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ImageStoreContent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.StoreFiles != null)
            {
                writer.WriteEnumerableProperty(obj.StoreFiles, "StoreFiles", FileInfoConverter.Serialize);
            }

            if (obj.StoreFolders != null)
            {
                writer.WriteEnumerableProperty(obj.StoreFolders, "StoreFolders", FolderInfoConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
