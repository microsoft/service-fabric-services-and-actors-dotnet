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
    /// Converter for <see cref="FileInfo" />.
    /// </summary>
    internal class FileInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static FileInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static FileInfo GetFromJsonProperties(JsonReader reader)
        {
            var fileSize = default(string);
            var fileVersion = default(FileVersion);
            var modifiedDate = default(DateTime?);
            var storeRelativePath = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("FileSize", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    fileSize = reader.ReadValueAsString();
                }
                else if (string.Compare("FileVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    fileVersion = FileVersionConverter.Deserialize(reader);
                }
                else if (string.Compare("ModifiedDate", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    modifiedDate = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("StoreRelativePath", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    storeRelativePath = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new FileInfo(
                fileSize: fileSize,
                fileVersion: fileVersion,
                modifiedDate: modifiedDate,
                storeRelativePath: storeRelativePath);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, FileInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.FileSize != null)
            {
                writer.WriteProperty(obj.FileSize, "FileSize", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FileVersion != null)
            {
                writer.WriteProperty(obj.FileVersion, "FileVersion", FileVersionConverter.Serialize);
            }

            if (obj.ModifiedDate != null)
            {
                writer.WriteProperty(obj.ModifiedDate, "ModifiedDate", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.StoreRelativePath != null)
            {
                writer.WriteProperty(obj.StoreRelativePath, "StoreRelativePath", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
