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
    /// Converter for <see cref="ImageStoreCopyDescription" />.
    /// </summary>
    internal class ImageStoreCopyDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ImageStoreCopyDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ImageStoreCopyDescription GetFromJsonProperties(JsonReader reader)
        {
            var remoteSource = default(string);
            var remoteDestination = default(string);
            var skipFiles = default(IEnumerable<string>);
            var checkMarkFile = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("RemoteSource", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    remoteSource = reader.ReadValueAsString();
                }
                else if (string.Compare("RemoteDestination", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    remoteDestination = reader.ReadValueAsString();
                }
                else if (string.Compare("SkipFiles", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    skipFiles = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else if (string.Compare("CheckMarkFile", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    checkMarkFile = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ImageStoreCopyDescription(
                remoteSource: remoteSource,
                remoteDestination: remoteDestination,
                skipFiles: skipFiles,
                checkMarkFile: checkMarkFile);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ImageStoreCopyDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.RemoteSource, "RemoteSource", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.RemoteDestination, "RemoteDestination", JsonWriterExtensions.WriteStringValue);
            if (obj.SkipFiles != null)
            {
                writer.WriteEnumerableProperty(obj.SkipFiles, "SkipFiles", (w, v) => writer.WriteStringValue(v));
            }

            if (obj.CheckMarkFile != null)
            {
                writer.WriteProperty(obj.CheckMarkFile, "CheckMarkFile", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
