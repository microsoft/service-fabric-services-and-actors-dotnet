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
    /// Converter for <see cref="NodeEvent" />.
    /// </summary>
    internal class NodeEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var nodeName = default(NodeName);

            do
            {
                var propName = reader.ReadPropertyName();
                if (propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
                {
                    var propValue = reader.ReadValueAsString();

                    if (propValue.Equals("NodeAborted", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeAbortedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeAddedToCluster", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeAddedToClusterEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeClosed", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeClosedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeDeactivateCompleted", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeDeactivateCompletedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeDeactivateStarted", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeDeactivateStartedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeDown", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeDownEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeNewHealthReport", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeNewHealthReportEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeHealthReportExpired", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeOpenSucceeded", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeOpenSucceededEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeOpenFailed", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeOpenFailedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeRemovedFromCluster", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeRemovedFromClusterEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeUp", StringComparison.OrdinalIgnoreCase))
                    {
                        return NodeUpEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ChaosNodeRestartScheduled", StringComparison.OrdinalIgnoreCase))
                    {
                        return ChaosNodeRestartScheduledEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("NodeEvent", StringComparison.OrdinalIgnoreCase))
                    {
                        // kind specified as same type, deserialize using properties.
                    }
                    else
                    {
                        throw new InvalidOperationException("Unknown Discriminator.");
                    }
                }
                else
                {
                    if (string.Compare("EventInstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        eventInstanceId = reader.ReadValueAsGuid();
                    }
                    else if (string.Compare("Category", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        category = reader.ReadValueAsString();
                    }
                    else if (string.Compare("TimeStamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        timeStamp = reader.ReadValueAsDateTime();
                    }
                    else if (string.Compare("HasCorrelatedEvents", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        hasCorrelatedEvents = reader.ReadValueAsBool();
                    }
                    else if (string.Compare("NodeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        nodeName = NodeNameConverter.Deserialize(reader);
                    }
                    else
                    {
                        reader.SkipPropertyValue();
                    }
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeEvent(
                kind: Common.FabricEventKind.NodeEvent,
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                nodeName: nodeName);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.NodeName, "NodeName", NodeNameConverter.Serialize);
            if (obj.Category != null)
            {
                writer.WriteProperty(obj.Category, "Category", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HasCorrelatedEvents != null)
            {
                writer.WriteProperty(obj.HasCorrelatedEvents, "HasCorrelatedEvents", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
