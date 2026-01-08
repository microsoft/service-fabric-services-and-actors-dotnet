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
    /// Converter for <see cref="UnprovisionApplicationTypeDescriptionInfo" />.
    /// </summary>
    internal class UnprovisionApplicationTypeDescriptionInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static UnprovisionApplicationTypeDescriptionInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static UnprovisionApplicationTypeDescriptionInfo GetFromJsonProperties(JsonReader reader)
        {
            var applicationTypeVersion = default(string);
            var async = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ApplicationTypeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationTypeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("Async", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    async = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new UnprovisionApplicationTypeDescriptionInfo(
                applicationTypeVersion: applicationTypeVersion,
                async: async);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, UnprovisionApplicationTypeDescriptionInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ApplicationTypeVersion, "ApplicationTypeVersion", JsonWriterExtensions.WriteStringValue);
            if (obj.Async != null)
            {
                writer.WriteProperty(obj.Async, "Async", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
