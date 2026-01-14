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
    /// Converter for <see cref="UpgradeOrchestrationServiceStateSummary" />.
    /// </summary>
    internal class UpgradeOrchestrationServiceStateSummaryConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static UpgradeOrchestrationServiceStateSummary Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static UpgradeOrchestrationServiceStateSummary GetFromJsonProperties(JsonReader reader)
        {
            var currentCodeVersion = default(string);
            var currentManifestVersion = default(string);
            var targetCodeVersion = default(string);
            var targetManifestVersion = default(string);
            var pendingUpgradeType = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("CurrentCodeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentCodeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("CurrentManifestVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentManifestVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("TargetCodeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    targetCodeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("TargetManifestVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    targetManifestVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("PendingUpgradeType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pendingUpgradeType = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new UpgradeOrchestrationServiceStateSummary(
                currentCodeVersion: currentCodeVersion,
                currentManifestVersion: currentManifestVersion,
                targetCodeVersion: targetCodeVersion,
                targetManifestVersion: targetManifestVersion,
                pendingUpgradeType: pendingUpgradeType);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, UpgradeOrchestrationServiceStateSummary obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.CurrentCodeVersion != null)
            {
                writer.WriteProperty(obj.CurrentCodeVersion, "CurrentCodeVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CurrentManifestVersion != null)
            {
                writer.WriteProperty(obj.CurrentManifestVersion, "CurrentManifestVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.TargetCodeVersion != null)
            {
                writer.WriteProperty(obj.TargetCodeVersion, "TargetCodeVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.TargetManifestVersion != null)
            {
                writer.WriteProperty(obj.TargetManifestVersion, "TargetManifestVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PendingUpgradeType != null)
            {
                writer.WriteProperty(obj.PendingUpgradeType, "PendingUpgradeType", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
