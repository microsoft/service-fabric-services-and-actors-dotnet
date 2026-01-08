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
    /// Converter for <see cref="AddRemoveReplicaScalingMechanism" />.
    /// </summary>
    internal class AddRemoveReplicaScalingMechanismConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AddRemoveReplicaScalingMechanism Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AddRemoveReplicaScalingMechanism GetFromJsonProperties(JsonReader reader)
        {
            var minCount = default(int?);
            var maxCount = default(int?);
            var scaleIncrement = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("minCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("maxCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("scaleIncrement", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scaleIncrement = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new AddRemoveReplicaScalingMechanism(
                minCount: minCount,
                maxCount: maxCount,
                scaleIncrement: scaleIncrement);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AddRemoveReplicaScalingMechanism obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "kind", AutoScalingMechanismKindConverter.Serialize);
            writer.WriteProperty(obj.MinCount, "minCount", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.MaxCount, "maxCount", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.ScaleIncrement, "scaleIncrement", JsonWriterExtensions.WriteIntValue);
            writer.WriteEndObject();
        }
    }
}
