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
    /// Converter for <see cref="ClusterHealthChunkQueryDescription" />.
    /// </summary>
    internal class ClusterHealthChunkQueryDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterHealthChunkQueryDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterHealthChunkQueryDescription GetFromJsonProperties(JsonReader reader)
        {
            var nodeFilters = default(IEnumerable<NodeHealthStateFilter>);
            var applicationFilters = default(IEnumerable<ApplicationHealthStateFilter>);
            var clusterHealthPolicy = default(ClusterHealthPolicy);
            var applicationHealthPolicies = default(ApplicationHealthPolicies);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("NodeFilters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeFilters = reader.ReadList(NodeHealthStateFilterConverter.Deserialize);
                }
                else if (string.Compare("ApplicationFilters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationFilters = reader.ReadList(ApplicationHealthStateFilterConverter.Deserialize);
                }
                else if (string.Compare("ClusterHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterHealthPolicy = ClusterHealthPolicyConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationHealthPolicies", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthPolicies = ApplicationHealthPoliciesConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ClusterHealthChunkQueryDescription(
                nodeFilters: nodeFilters,
                applicationFilters: applicationFilters,
                clusterHealthPolicy: clusterHealthPolicy,
                applicationHealthPolicies: applicationHealthPolicies);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ClusterHealthChunkQueryDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.NodeFilters != null)
            {
                writer.WriteEnumerableProperty(obj.NodeFilters, "NodeFilters", NodeHealthStateFilterConverter.Serialize);
            }

            if (obj.ApplicationFilters != null)
            {
                writer.WriteEnumerableProperty(obj.ApplicationFilters, "ApplicationFilters", ApplicationHealthStateFilterConverter.Serialize);
            }

            if (obj.ClusterHealthPolicy != null)
            {
                writer.WriteProperty(obj.ClusterHealthPolicy, "ClusterHealthPolicy", ClusterHealthPolicyConverter.Serialize);
            }

            if (obj.ApplicationHealthPolicies != null)
            {
                writer.WriteProperty(obj.ApplicationHealthPolicies, "ApplicationHealthPolicies", ApplicationHealthPoliciesConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
