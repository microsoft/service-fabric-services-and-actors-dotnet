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
    /// Converter for <see cref="ClusterHealthPolicies" />.
    /// </summary>
    internal class ClusterHealthPoliciesConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterHealthPolicies Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterHealthPolicies GetFromJsonProperties(JsonReader reader)
        {
            var applicationHealthPolicyMap = default(IEnumerable<ApplicationHealthPolicyMapItem>);
            var clusterHealthPolicy = default(ClusterHealthPolicy);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ApplicationHealthPolicyMap", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthPolicyMap = reader.ReadList(ApplicationHealthPolicyMapItemConverter.Deserialize);
                }
                else if (string.Compare("ClusterHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterHealthPolicy = ClusterHealthPolicyConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ClusterHealthPolicies(
                applicationHealthPolicyMap: applicationHealthPolicyMap,
                clusterHealthPolicy: clusterHealthPolicy);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ClusterHealthPolicies obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            if (obj.ApplicationHealthPolicyMap != null)
            {
                writer.WriteEnumerableProperty(obj.ApplicationHealthPolicyMap, "ApplicationHealthPolicyMap", ApplicationHealthPolicyMapItemConverter.Serialize);
            }

            if (obj.ClusterHealthPolicy != null)
            {
                writer.WriteProperty(obj.ClusterHealthPolicy, "ClusterHealthPolicy", ClusterHealthPolicyConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
