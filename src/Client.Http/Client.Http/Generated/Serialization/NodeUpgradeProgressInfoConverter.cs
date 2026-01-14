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
    /// Converter for <see cref="NodeUpgradeProgressInfo" />.
    /// </summary>
    internal class NodeUpgradeProgressInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeUpgradeProgressInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NodeUpgradeProgressInfo GetFromJsonProperties(JsonReader reader)
        {
            var nodeName = default(NodeName);
            var upgradePhase = default(NodeUpgradePhase?);
            var pendingSafetyChecks = default(IEnumerable<SafetyCheckWrapper>);
            var upgradeDuration = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("NodeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeName = NodeNameConverter.Deserialize(reader);
                }
                else if (string.Compare("UpgradePhase", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradePhase = NodeUpgradePhaseConverter.Deserialize(reader);
                }
                else if (string.Compare("PendingSafetyChecks", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pendingSafetyChecks = reader.ReadList(SafetyCheckWrapperConverter.Deserialize);
                }
                else if (string.Compare("UpgradeDuration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeDuration = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeUpgradeProgressInfo(
                nodeName: nodeName,
                upgradePhase: upgradePhase,
                pendingSafetyChecks: pendingSafetyChecks,
                upgradeDuration: upgradeDuration);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeUpgradeProgressInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UpgradePhase, "UpgradePhase", NodeUpgradePhaseConverter.Serialize);
            if (obj.NodeName != null)
            {
                writer.WriteProperty(obj.NodeName, "NodeName", NodeNameConverter.Serialize);
            }

            if (obj.PendingSafetyChecks != null)
            {
                writer.WriteEnumerableProperty(obj.PendingSafetyChecks, "PendingSafetyChecks", SafetyCheckWrapperConverter.Serialize);
            }

            if (obj.UpgradeDuration != null)
            {
                writer.WriteProperty(obj.UpgradeDuration, "UpgradeDuration", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
