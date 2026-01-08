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
    /// Converter for <see cref="BasicRetentionPolicyDescription" />.
    /// </summary>
    internal class BasicRetentionPolicyDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BasicRetentionPolicyDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static BasicRetentionPolicyDescription GetFromJsonProperties(JsonReader reader)
        {
            var retentionDuration = default(TimeSpan?);
            var minimumNumberOfBackups = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("RetentionDuration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    retentionDuration = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("MinimumNumberOfBackups", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minimumNumberOfBackups = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new BasicRetentionPolicyDescription(
                retentionDuration: retentionDuration,
                minimumNumberOfBackups: minimumNumberOfBackups);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, BasicRetentionPolicyDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.RetentionPolicyType, "RetentionPolicyType", RetentionPolicyTypeConverter.Serialize);
            writer.WriteProperty(obj.RetentionDuration, "RetentionDuration", JsonWriterExtensions.WriteTimeSpanValue);
            if (obj.MinimumNumberOfBackups != null)
            {
                writer.WriteProperty(obj.MinimumNumberOfBackups, "MinimumNumberOfBackups", JsonWriterExtensions.WriteIntValue);
            }

            writer.WriteEndObject();
        }
    }
}
