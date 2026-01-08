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
    /// Converter for <see cref="CurrentUpgradeUnitsProgressInfo" />.
    /// </summary>
    internal class CurrentUpgradeUnitsProgressInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static CurrentUpgradeUnitsProgressInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static CurrentUpgradeUnitsProgressInfo GetFromJsonProperties(JsonReader reader)
        {
            var domainName = default(string);
            var nodeUpgradeProgressList = default(IEnumerable<NodeUpgradeProgressInfo>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("DomainName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    domainName = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeUpgradeProgressList", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeUpgradeProgressList = reader.ReadList(NodeUpgradeProgressInfoConverter.Deserialize);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new CurrentUpgradeUnitsProgressInfo(
                domainName: domainName,
                nodeUpgradeProgressList: nodeUpgradeProgressList);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, CurrentUpgradeUnitsProgressInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.DomainName != null)
            {
                writer.WriteProperty(obj.DomainName, "DomainName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.NodeUpgradeProgressList != null)
            {
                writer.WriteEnumerableProperty(obj.NodeUpgradeProgressList, "NodeUpgradeProgressList", NodeUpgradeProgressInfoConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
