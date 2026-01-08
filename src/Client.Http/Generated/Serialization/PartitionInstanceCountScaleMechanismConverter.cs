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
    /// Converter for <see cref="PartitionInstanceCountScaleMechanism" />.
    /// </summary>
    internal class PartitionInstanceCountScaleMechanismConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionInstanceCountScaleMechanism Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionInstanceCountScaleMechanism GetFromJsonProperties(JsonReader reader)
        {
            var minInstanceCount = default(int?);
            var maxInstanceCount = default(int?);
            var scaleIncrement = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("MinInstanceCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minInstanceCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaxInstanceCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxInstanceCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("ScaleIncrement", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    scaleIncrement = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PartitionInstanceCountScaleMechanism(
                minInstanceCount: minInstanceCount,
                maxInstanceCount: maxInstanceCount,
                scaleIncrement: scaleIncrement);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionInstanceCountScaleMechanism obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ScalingMechanismKindConverter.Serialize);
            writer.WriteProperty(obj.MinInstanceCount, "MinInstanceCount", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.MaxInstanceCount, "MaxInstanceCount", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.ScaleIncrement, "ScaleIncrement", JsonWriterExtensions.WriteIntValue);
            writer.WriteEndObject();
        }
    }
}
