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
    /// Converter for <see cref="Chaos" />.
    /// </summary>
    internal class ChaosConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static Chaos Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static Chaos GetFromJsonProperties(JsonReader reader)
        {
            var chaosParameters = default(ChaosParameters);
            var status = default(ChaosStatus?);
            var scheduleStatus = default(ChaosScheduleStatus?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ChaosParameters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    chaosParameters = ChaosParametersConverter.Deserialize(reader);
                }
                else if (string.Compare("Status", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    status = ChaosStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("ScheduleStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scheduleStatus = ChaosScheduleStatusConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new Chaos(
                chaosParameters: chaosParameters,
                status: status,
                scheduleStatus: scheduleStatus);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, Chaos obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Status, "Status", ChaosStatusConverter.Serialize);
            writer.WriteProperty(obj.ScheduleStatus, "ScheduleStatus", ChaosScheduleStatusConverter.Serialize);
            if (obj.ChaosParameters != null)
            {
                writer.WriteProperty(obj.ChaosParameters, "ChaosParameters", ChaosParametersConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
