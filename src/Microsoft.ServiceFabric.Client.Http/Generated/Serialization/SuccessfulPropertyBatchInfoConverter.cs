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
    /// Converter for <see cref="SuccessfulPropertyBatchInfo" />.
    /// </summary>
    internal class SuccessfulPropertyBatchInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static SuccessfulPropertyBatchInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static SuccessfulPropertyBatchInfo GetFromJsonProperties(JsonReader reader)
        {
            var properties = default(IReadOnlyDictionary<string, PropertyInfo>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Properties", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    properties = reader.ReadDictionary(PropertyInfoConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new SuccessfulPropertyBatchInfo(
                properties: properties);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, SuccessfulPropertyBatchInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", PropertyBatchInfoKindConverter.Serialize);
            if (obj.Properties != null)
            {
                writer.WriteDictionaryProperty(obj.Properties, "Properties", PropertyInfoConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
