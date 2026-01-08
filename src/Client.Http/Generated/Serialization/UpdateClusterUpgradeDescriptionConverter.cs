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
    /// Converter for <see cref="UpdateClusterUpgradeDescription" />.
    /// </summary>
    internal class UpdateClusterUpgradeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static UpdateClusterUpgradeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static UpdateClusterUpgradeDescription GetFromJsonProperties(JsonReader reader)
        {
            var upgradeKind = default(UpgradeType?);
            var updateDescription = default(RollingUpgradeUpdateDescription);
            var clusterHealthPolicy = default(ClusterHealthPolicy);
            var enableDeltaHealthEvaluation = default(bool?);
            var clusterUpgradeHealthPolicy = default(ClusterUpgradeHealthPolicyObject);
            var applicationHealthPolicyMap = default(ApplicationHealthPolicies);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("UpgradeKind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeKind = UpgradeTypeConverter.Deserialize(reader);
                }
                else if (string.Compare("UpdateDescription", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    updateDescription = RollingUpgradeUpdateDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("ClusterHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterHealthPolicy = ClusterHealthPolicyConverter.Deserialize(reader);
                }
                else if (string.Compare("EnableDeltaHealthEvaluation", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    enableDeltaHealthEvaluation = reader.ReadValueAsBool();
                }
                else if (string.Compare("ClusterUpgradeHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterUpgradeHealthPolicy = ClusterUpgradeHealthPolicyObjectConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationHealthPolicyMap", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthPolicyMap = ApplicationHealthPoliciesConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new UpdateClusterUpgradeDescription(
                upgradeKind: upgradeKind,
                updateDescription: updateDescription,
                clusterHealthPolicy: clusterHealthPolicy,
                enableDeltaHealthEvaluation: enableDeltaHealthEvaluation,
                clusterUpgradeHealthPolicy: clusterUpgradeHealthPolicy,
                applicationHealthPolicyMap: applicationHealthPolicyMap);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, UpdateClusterUpgradeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UpgradeKind, "UpgradeKind", UpgradeTypeConverter.Serialize);
            if (obj.UpdateDescription != null)
            {
                writer.WriteProperty(obj.UpdateDescription, "UpdateDescription", RollingUpgradeUpdateDescriptionConverter.Serialize);
            }

            if (obj.ClusterHealthPolicy != null)
            {
                writer.WriteProperty(obj.ClusterHealthPolicy, "ClusterHealthPolicy", ClusterHealthPolicyConverter.Serialize);
            }

            if (obj.EnableDeltaHealthEvaluation != null)
            {
                writer.WriteProperty(obj.EnableDeltaHealthEvaluation, "EnableDeltaHealthEvaluation", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.ClusterUpgradeHealthPolicy != null)
            {
                writer.WriteProperty(obj.ClusterUpgradeHealthPolicy, "ClusterUpgradeHealthPolicy", ClusterUpgradeHealthPolicyObjectConverter.Serialize);
            }

            if (obj.ApplicationHealthPolicyMap != null)
            {
                writer.WriteProperty(obj.ApplicationHealthPolicyMap, "ApplicationHealthPolicyMap", ApplicationHealthPoliciesConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
