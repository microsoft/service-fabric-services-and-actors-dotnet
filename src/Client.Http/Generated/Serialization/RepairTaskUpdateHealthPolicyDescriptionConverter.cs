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
    /// Converter for <see cref="RepairTaskUpdateHealthPolicyDescription" />.
    /// </summary>
    internal class RepairTaskUpdateHealthPolicyDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RepairTaskUpdateHealthPolicyDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RepairTaskUpdateHealthPolicyDescription GetFromJsonProperties(JsonReader reader)
        {
            var taskId = default(string);
            var version = default(string);
            var performPreparingHealthCheck = default(bool?);
            var performRestoringHealthCheck = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("TaskId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    taskId = reader.ReadValueAsString();
                }
                else if (string.Compare("Version", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    version = reader.ReadValueAsString();
                }
                else if (string.Compare("PerformPreparingHealthCheck", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    performPreparingHealthCheck = reader.ReadValueAsBool();
                }
                else if (string.Compare("PerformRestoringHealthCheck", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    performRestoringHealthCheck = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RepairTaskUpdateHealthPolicyDescription(
                taskId: taskId,
                version: version,
                performPreparingHealthCheck: performPreparingHealthCheck,
                performRestoringHealthCheck: performRestoringHealthCheck);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RepairTaskUpdateHealthPolicyDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.TaskId, "TaskId", JsonWriterExtensions.WriteStringValue);
            if (obj.Version != null)
            {
                writer.WriteProperty(obj.Version, "Version", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PerformPreparingHealthCheck != null)
            {
                writer.WriteProperty(obj.PerformPreparingHealthCheck, "PerformPreparingHealthCheck", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.PerformRestoringHealthCheck != null)
            {
                writer.WriteProperty(obj.PerformRestoringHealthCheck, "PerformRestoringHealthCheck", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
