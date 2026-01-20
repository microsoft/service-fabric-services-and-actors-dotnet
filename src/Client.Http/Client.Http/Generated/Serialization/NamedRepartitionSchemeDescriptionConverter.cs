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
    /// Converter for <see cref="NamedRepartitionSchemeDescription" />.
    /// </summary>
    internal class NamedRepartitionSchemeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NamedRepartitionSchemeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NamedRepartitionSchemeDescription GetFromJsonProperties(JsonReader reader)
        {
            var namesToAdd = default(IEnumerable<string>);
            var namesToRemove = default(IEnumerable<string>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("NamesToAdd", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    namesToAdd = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else if (string.Compare("NamesToRemove", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    namesToRemove = reader.ReadList(JsonReaderExtensions.ReadValueAsString);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NamedRepartitionSchemeDescription(
                namesToAdd: namesToAdd,
                namesToRemove: namesToRemove);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NamedRepartitionSchemeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", RepartitionSchemeConverter.Serialize);
            if (obj.NamesToAdd != null)
            {
                writer.WriteEnumerableProperty(obj.NamesToAdd, "NamesToAdd", (w, v) => writer.WriteStringValue(v));
            }

            if (obj.NamesToRemove != null)
            {
                writer.WriteEnumerableProperty(obj.NamesToRemove, "NamesToRemove", (w, v) => writer.WriteStringValue(v));
            }

            writer.WriteEndObject();
        }
    }
}
