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
    /// Converter for <see cref="NodeRepairImpactDescription" />.
    /// </summary>
    internal class NodeRepairImpactDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static NodeRepairImpactDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static NodeRepairImpactDescription GetFromJsonProperties(JsonReader reader)
        {
            var nodeImpactList = default(IEnumerable<NodeImpact>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("NodeImpactList", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeImpactList = reader.ReadList(NodeImpactConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new NodeRepairImpactDescription(
                nodeImpactList: nodeImpactList);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, NodeRepairImpactDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", RepairImpactKindConverter.Serialize);
            if (obj.NodeImpactList != null)
            {
                writer.WriteEnumerableProperty(obj.NodeImpactList, "NodeImpactList", NodeImpactConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
