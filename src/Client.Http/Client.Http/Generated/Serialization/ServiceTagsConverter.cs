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
    /// Converter for <see cref="ServiceTags" />.
    /// </summary>
    internal class ServiceTagsConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceTags Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ServiceTags GetFromJsonProperties(JsonReader reader)
        {
            var tagsRequiredToPlace = default(IEnumerable<string>);
            var tagsRequiredToRun = default(IEnumerable<string>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("TagsRequiredToPlace", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tagsRequiredToPlace = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else if (string.Compare("TagsRequiredToRun", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    tagsRequiredToRun = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ServiceTags(
                tagsRequiredToPlace: tagsRequiredToPlace,
                tagsRequiredToRun: tagsRequiredToRun);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ServiceTags obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.TagsRequiredToPlace != null)
            {
                writer.WriteEnumerableProperty(obj.TagsRequiredToPlace, "TagsRequiredToPlace", (w, v) => writer.WriteStringValue(v));
            }

            if (obj.TagsRequiredToRun != null)
            {
                writer.WriteEnumerableProperty(obj.TagsRequiredToRun, "TagsRequiredToRun", (w, v) => writer.WriteStringValue(v));
            }

            writer.WriteEndObject();
        }
    }
}
