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
    /// Converter for <see cref="ContainerInstanceView" />.
    /// </summary>
    internal class ContainerInstanceViewConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerInstanceView Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ContainerInstanceView GetFromJsonProperties(JsonReader reader)
        {
            var restartCount = default(int?);
            var currentState = default(ContainerState);
            var previousState = default(ContainerState);
            var events = default(IEnumerable<ContainerEvent>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("restartCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restartCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("currentState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentState = ContainerStateConverter.Deserialize(reader);
                }
                else if (string.Compare("previousState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    previousState = ContainerStateConverter.Deserialize(reader);
                }
                else if (string.Compare("events", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    events = reader.ReadList(ContainerEventConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ContainerInstanceView(
                restartCount: restartCount,
                currentState: currentState,
                previousState: previousState,
                events: events);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ContainerInstanceView obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.RestartCount != null)
            {
                writer.WriteProperty(obj.RestartCount, "restartCount", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.CurrentState != null)
            {
                writer.WriteProperty(obj.CurrentState, "currentState", ContainerStateConverter.Serialize);
            }

            if (obj.PreviousState != null)
            {
                writer.WriteProperty(obj.PreviousState, "previousState", ContainerStateConverter.Serialize);
            }

            if (obj.Events != null)
            {
                writer.WriteEnumerableProperty(obj.Events, "events", ContainerEventConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
