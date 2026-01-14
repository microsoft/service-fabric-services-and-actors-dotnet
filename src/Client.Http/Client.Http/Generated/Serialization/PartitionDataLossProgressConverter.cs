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
    /// Converter for <see cref="PartitionDataLossProgress" />.
    /// </summary>
    internal class PartitionDataLossProgressConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionDataLossProgress Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static PartitionDataLossProgress GetFromJsonProperties(JsonReader reader)
        {
            var state = default(OperationState?);
            var invokeDataLossResult = default(InvokeDataLossResult);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("State", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    state = OperationStateConverter.Deserialize(reader);
                }
                else if (string.Compare("InvokeDataLossResult", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    invokeDataLossResult = InvokeDataLossResultConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PartitionDataLossProgress(
                state: state,
                invokeDataLossResult: invokeDataLossResult);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, PartitionDataLossProgress obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.State, "State", OperationStateConverter.Serialize);
            if (obj.InvokeDataLossResult != null)
            {
                writer.WriteProperty(obj.InvokeDataLossResult, "InvokeDataLossResult", InvokeDataLossResultConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
