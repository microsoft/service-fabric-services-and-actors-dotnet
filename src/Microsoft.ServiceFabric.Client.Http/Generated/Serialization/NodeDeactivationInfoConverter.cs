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
    /// Converter for <see cref="NodeDeactivationInfo" />.
    /// </summary>
    internal class NodeDeactivationInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeDeactivationInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NodeDeactivationInfo GetFromJsonProperties(JsonReader reader)
        {
            var nodeDeactivationIntent = default(NodeDeactivationIntent?);
            var nodeDeactivationStatus = default(NodeDeactivationStatus?);
            var nodeDeactivationTask = default(IEnumerable<NodeDeactivationTask>);
            var pendingSafetyChecks = default(IEnumerable<SafetyCheckWrapper>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("NodeDeactivationIntent", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeDeactivationIntent = NodeDeactivationIntentConverter.Deserialize(reader);
                }
                else if (string.Compare("NodeDeactivationStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeDeactivationStatus = NodeDeactivationStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("NodeDeactivationTask", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeDeactivationTask = reader.ReadList(NodeDeactivationTaskConverter.Deserialize);
                }
                else if (string.Compare("PendingSafetyChecks", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pendingSafetyChecks = reader.ReadList(SafetyCheckWrapperConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeDeactivationInfo(
                nodeDeactivationIntent: nodeDeactivationIntent,
                nodeDeactivationStatus: nodeDeactivationStatus,
                nodeDeactivationTask: nodeDeactivationTask,
                pendingSafetyChecks: pendingSafetyChecks);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeDeactivationInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.NodeDeactivationIntent, "NodeDeactivationIntent", NodeDeactivationIntentConverter.Serialize);
            writer.WriteProperty(obj.NodeDeactivationStatus, "NodeDeactivationStatus", NodeDeactivationStatusConverter.Serialize);
            if (obj.NodeDeactivationTask != null)
            {
                writer.WriteEnumerableProperty(obj.NodeDeactivationTask, "NodeDeactivationTask", NodeDeactivationTaskConverter.Serialize);
            }

            if (obj.PendingSafetyChecks != null)
            {
                writer.WriteEnumerableProperty(obj.PendingSafetyChecks, "PendingSafetyChecks", SafetyCheckWrapperConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
