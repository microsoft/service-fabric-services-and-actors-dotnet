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
    /// Converter for <see cref="ReconfigurationInformation" />.
    /// </summary>
    internal class ReconfigurationInformationConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ReconfigurationInformation Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ReconfigurationInformation GetFromJsonProperties(JsonReader reader)
        {
            var previousConfigurationRole = default(ReplicaRole?);
            var reconfigurationPhase = default(ReconfigurationPhase?);
            var reconfigurationType = default(ReconfigurationType?);
            var reconfigurationStartTimeUtc = default(DateTime?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PreviousConfigurationRole", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    previousConfigurationRole = ReplicaRoleConverter.Deserialize(reader);
                }
                else if (string.Compare("ReconfigurationPhase", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    reconfigurationPhase = ReconfigurationPhaseConverter.Deserialize(reader);
                }
                else if (string.Compare("ReconfigurationType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    reconfigurationType = ReconfigurationTypeConverter.Deserialize(reader);
                }
                else if (string.Compare("ReconfigurationStartTimeUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    reconfigurationStartTimeUtc = reader.ReadValueAsDateTime();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ReconfigurationInformation(
                previousConfigurationRole: previousConfigurationRole,
                reconfigurationPhase: reconfigurationPhase,
                reconfigurationType: reconfigurationType,
                reconfigurationStartTimeUtc: reconfigurationStartTimeUtc);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ReconfigurationInformation obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.PreviousConfigurationRole, "PreviousConfigurationRole", ReplicaRoleConverter.Serialize);
            writer.WriteProperty(obj.ReconfigurationPhase, "ReconfigurationPhase", ReconfigurationPhaseConverter.Serialize);
            writer.WriteProperty(obj.ReconfigurationType, "ReconfigurationType", ReconfigurationTypeConverter.Serialize);
            if (obj.ReconfigurationStartTimeUtc != null)
            {
                writer.WriteProperty(obj.ReconfigurationStartTimeUtc, "ReconfigurationStartTimeUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            writer.WriteEndObject();
        }
    }
}
