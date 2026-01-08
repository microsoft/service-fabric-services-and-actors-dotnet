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
    /// Converter for <see cref="ImageStoreInfo" />.
    /// </summary>
    internal class ImageStoreInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ImageStoreInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ImageStoreInfo GetFromJsonProperties(JsonReader reader)
        {
            var diskInfo = default(DiskInfo);
            var usedByMetadata = default(UsageInfo);
            var usedByStaging = default(UsageInfo);
            var usedByCopy = default(UsageInfo);
            var usedByRegister = default(UsageInfo);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("DiskInfo", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    diskInfo = DiskInfoConverter.Deserialize(reader);
                }
                else if (string.Compare("UsedByMetadata", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    usedByMetadata = UsageInfoConverter.Deserialize(reader);
                }
                else if (string.Compare("UsedByStaging", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    usedByStaging = UsageInfoConverter.Deserialize(reader);
                }
                else if (string.Compare("UsedByCopy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    usedByCopy = UsageInfoConverter.Deserialize(reader);
                }
                else if (string.Compare("UsedByRegister", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    usedByRegister = UsageInfoConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ImageStoreInfo(
                diskInfo: diskInfo,
                usedByMetadata: usedByMetadata,
                usedByStaging: usedByStaging,
                usedByCopy: usedByCopy,
                usedByRegister: usedByRegister);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ImageStoreInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.DiskInfo != null)
            {
                writer.WriteProperty(obj.DiskInfo, "DiskInfo", DiskInfoConverter.Serialize);
            }

            if (obj.UsedByMetadata != null)
            {
                writer.WriteProperty(obj.UsedByMetadata, "UsedByMetadata", UsageInfoConverter.Serialize);
            }

            if (obj.UsedByStaging != null)
            {
                writer.WriteProperty(obj.UsedByStaging, "UsedByStaging", UsageInfoConverter.Serialize);
            }

            if (obj.UsedByCopy != null)
            {
                writer.WriteProperty(obj.UsedByCopy, "UsedByCopy", UsageInfoConverter.Serialize);
            }

            if (obj.UsedByRegister != null)
            {
                writer.WriteProperty(obj.UsedByRegister, "UsedByRegister", UsageInfoConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
