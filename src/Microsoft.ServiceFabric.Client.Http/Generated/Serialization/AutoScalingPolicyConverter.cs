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
    /// Converter for <see cref="AutoScalingPolicy" />.
    /// </summary>
    internal class AutoScalingPolicyConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AutoScalingPolicy Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AutoScalingPolicy GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var trigger = default(AutoScalingTrigger);
            var mechanism = default(AutoScalingMechanism);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("trigger", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    trigger = AutoScalingTriggerConverter.Deserialize(reader);
                }
                else if (string.Compare("mechanism", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    mechanism = AutoScalingMechanismConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new AutoScalingPolicy(
                name: name,
                trigger: trigger,
                mechanism: mechanism);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AutoScalingPolicy obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Name, "name", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Trigger, "trigger", AutoScalingTriggerConverter.Serialize);
            writer.WriteProperty(obj.Mechanism, "mechanism", AutoScalingMechanismConverter.Serialize);
            writer.WriteEndObject();
        }
    }
}
