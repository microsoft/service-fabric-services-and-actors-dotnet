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
    /// Converter for <see cref="ClusterUpgradeHealthPolicyObject" />.
    /// </summary>
    internal class ClusterUpgradeHealthPolicyObjectConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterUpgradeHealthPolicyObject Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterUpgradeHealthPolicyObject GetFromJsonProperties(JsonReader reader)
        {
            var maxPercentDeltaUnhealthyNodes = default(int?);
            var maxPercentUpgradeDomainDeltaUnhealthyNodes = default(int?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("MaxPercentDeltaUnhealthyNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentDeltaUnhealthyNodes = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaxPercentUpgradeDomainDeltaUnhealthyNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentUpgradeDomainDeltaUnhealthyNodes = reader.ReadValueAsInt();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ClusterUpgradeHealthPolicyObject(
                maxPercentDeltaUnhealthyNodes: maxPercentDeltaUnhealthyNodes,
                maxPercentUpgradeDomainDeltaUnhealthyNodes: maxPercentUpgradeDomainDeltaUnhealthyNodes);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ClusterUpgradeHealthPolicyObject obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.MaxPercentDeltaUnhealthyNodes != null)
            {
                writer.WriteProperty(obj.MaxPercentDeltaUnhealthyNodes, "MaxPercentDeltaUnhealthyNodes", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MaxPercentUpgradeDomainDeltaUnhealthyNodes != null)
            {
                writer.WriteProperty(obj.MaxPercentUpgradeDomainDeltaUnhealthyNodes, "MaxPercentUpgradeDomainDeltaUnhealthyNodes", JsonWriterExtensions.WriteIntValue);
            }

            writer.WriteEndObject();
        }
    }
}
