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
    /// Converter for <see cref="ContainerState" />.
    /// </summary>
    internal class ContainerStateConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerState Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerState GetFromJsonProperties(JsonReader reader)
        {
            var state = default(string);
            var startTime = default(DateTime?);
            var exitCode = default(string);
            var finishTime = default(DateTime?);
            var detailStatus = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("state", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    state = reader.ReadValueAsString();
                }
                else if (string.Compare("startTime", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    startTime = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("exitCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exitCode = reader.ReadValueAsString();
                }
                else if (string.Compare("finishTime", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    finishTime = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("detailStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    detailStatus = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ContainerState(
                state: state,
                startTime: startTime,
                exitCode: exitCode,
                finishTime: finishTime,
                detailStatus: detailStatus);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ContainerState obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.State != null)
            {
                writer.WriteProperty(obj.State, "state", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.StartTime != null)
            {
                writer.WriteProperty(obj.StartTime, "startTime", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.ExitCode != null)
            {
                writer.WriteProperty(obj.ExitCode, "exitCode", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FinishTime != null)
            {
                writer.WriteProperty(obj.FinishTime, "finishTime", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.DetailStatus != null)
            {
                writer.WriteProperty(obj.DetailStatus, "detailStatus", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
