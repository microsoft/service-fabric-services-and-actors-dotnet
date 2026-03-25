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
    /// Converter for <see cref="DisableServiceDescription" />.
    /// </summary>
    internal class DisableServiceDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static DisableServiceDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static DisableServiceDescription GetFromJsonProperties(JsonReader reader)
        {
            var disableServiceFlag = default(DisableServiceFlag?);
            var forceDisable = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("DisableServiceFlag", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    disableServiceFlag = DisableServiceFlagConverter.Deserialize(reader);
                }
                else if (string.Compare("ForceDisable", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    forceDisable = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new DisableServiceDescription(
                disableServiceFlag: disableServiceFlag,
                forceDisable: forceDisable);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, DisableServiceDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.DisableServiceFlag, "DisableServiceFlag", DisableServiceFlagConverter.Serialize);
            if (obj.ForceDisable != null)
            {
                writer.WriteProperty(obj.ForceDisable, "ForceDisable", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
