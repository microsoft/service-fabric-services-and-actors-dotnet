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
    /// Converter for <see cref="AddRemoveIncrementalNamedPartitionScalingMechanism" />.
    /// </summary>
    internal class AddRemoveIncrementalNamedPartitionScalingMechanismConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static AddRemoveIncrementalNamedPartitionScalingMechanism Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static AddRemoveIncrementalNamedPartitionScalingMechanism GetFromJsonProperties(JsonReader reader)
        {
            var minPartitionCount = default(int?);
            var maxPartitionCount = default(int?);
            var scaleIncrement = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("MinPartitionCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    minPartitionCount = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaxPartitionCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPartitionCount = reader.ReadValueAsInt();
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

            return new AddRemoveIncrementalNamedPartitionScalingMechanism(
                minPartitionCount: minPartitionCount,
                maxPartitionCount: maxPartitionCount,
                scaleIncrement: scaleIncrement);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, AddRemoveIncrementalNamedPartitionScalingMechanism obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", ScalingMechanismKindConverter.Serialize);
            writer.WriteProperty(obj.MinPartitionCount, "MinPartitionCount", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.MaxPartitionCount, "MaxPartitionCount", JsonWriterExtensions.WriteIntValue);
            writer.WriteProperty(obj.ScaleIncrement, "ScaleIncrement", JsonWriterExtensions.WriteIntValue);
            writer.WriteEndObject();
        }
    }
}
