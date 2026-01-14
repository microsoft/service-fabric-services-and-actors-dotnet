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
    /// Converter for <see cref="ClusterConfigurationUpgradeDescription" />.
    /// </summary>
    internal class ClusterConfigurationUpgradeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterConfigurationUpgradeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ClusterConfigurationUpgradeDescription GetFromJsonProperties(JsonReader reader)
        {
            var clusterConfig = default(string);
            var healthCheckRetryTimeout = default(TimeSpan?);
            var healthCheckWaitDurationInSeconds = default(TimeSpan?);
            var healthCheckStableDurationInSeconds = default(TimeSpan?);
            var upgradeDomainTimeoutInSeconds = default(TimeSpan?);
            var upgradeTimeoutInSeconds = default(TimeSpan?);
            var maxPercentUnhealthyApplications = default(int?);
            var maxPercentUnhealthyNodes = default(int?);
            var maxPercentDeltaUnhealthyNodes = default(int?);
            var maxPercentUpgradeDomainDeltaUnhealthyNodes = default(int?);
            var applicationHealthPolicies = default(ApplicationHealthPolicies);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ClusterConfig", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    clusterConfig = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthCheckRetryTimeout", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthCheckRetryTimeout = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("HealthCheckWaitDurationInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthCheckWaitDurationInSeconds = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("HealthCheckStableDurationInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthCheckStableDurationInSeconds = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("UpgradeDomainTimeoutInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeDomainTimeoutInSeconds = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("UpgradeTimeoutInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeTimeoutInSeconds = reader.ReadValueAsTimeSpan();
                }
                else if (string.Compare("MaxPercentUnhealthyApplications", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentUnhealthyApplications = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaxPercentUnhealthyNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentUnhealthyNodes = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaxPercentDeltaUnhealthyNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentDeltaUnhealthyNodes = reader.ReadValueAsInt();
                }
                else if (string.Compare("MaxPercentUpgradeDomainDeltaUnhealthyNodes", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxPercentUpgradeDomainDeltaUnhealthyNodes = reader.ReadValueAsInt();
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

            return new ClusterConfigurationUpgradeDescription(
                clusterConfig: clusterConfig,
                healthCheckRetryTimeout: healthCheckRetryTimeout,
                healthCheckWaitDurationInSeconds: healthCheckWaitDurationInSeconds,
                healthCheckStableDurationInSeconds: healthCheckStableDurationInSeconds,
                upgradeDomainTimeoutInSeconds: upgradeDomainTimeoutInSeconds,
                upgradeTimeoutInSeconds: upgradeTimeoutInSeconds,
                maxPercentUnhealthyApplications: maxPercentUnhealthyApplications,
                maxPercentUnhealthyNodes: maxPercentUnhealthyNodes,
                maxPercentDeltaUnhealthyNodes: maxPercentDeltaUnhealthyNodes,
                maxPercentUpgradeDomainDeltaUnhealthyNodes: maxPercentUpgradeDomainDeltaUnhealthyNodes,
                applicationHealthPolicies: applicationHealthPolicies);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ClusterConfigurationUpgradeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ClusterConfig, "ClusterConfig", JsonWriterExtensions.WriteStringValue);
            if (obj.HealthCheckRetryTimeout != null)
            {
                writer.WriteProperty(obj.HealthCheckRetryTimeout, "HealthCheckRetryTimeout", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.HealthCheckWaitDurationInSeconds != null)
            {
                writer.WriteProperty(obj.HealthCheckWaitDurationInSeconds, "HealthCheckWaitDurationInSeconds", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.HealthCheckStableDurationInSeconds != null)
            {
                writer.WriteProperty(obj.HealthCheckStableDurationInSeconds, "HealthCheckStableDurationInSeconds", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.UpgradeDomainTimeoutInSeconds != null)
            {
                writer.WriteProperty(obj.UpgradeDomainTimeoutInSeconds, "UpgradeDomainTimeoutInSeconds", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.UpgradeTimeoutInSeconds != null)
            {
                writer.WriteProperty(obj.UpgradeTimeoutInSeconds, "UpgradeTimeoutInSeconds", JsonWriterExtensions.WriteTimeSpanValue);
            }

            if (obj.MaxPercentUnhealthyApplications != null)
            {
                writer.WriteProperty(obj.MaxPercentUnhealthyApplications, "MaxPercentUnhealthyApplications", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MaxPercentUnhealthyNodes != null)
            {
                writer.WriteProperty(obj.MaxPercentUnhealthyNodes, "MaxPercentUnhealthyNodes", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MaxPercentDeltaUnhealthyNodes != null)
            {
                writer.WriteProperty(obj.MaxPercentDeltaUnhealthyNodes, "MaxPercentDeltaUnhealthyNodes", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.MaxPercentUpgradeDomainDeltaUnhealthyNodes != null)
            {
                writer.WriteProperty(obj.MaxPercentUpgradeDomainDeltaUnhealthyNodes, "MaxPercentUpgradeDomainDeltaUnhealthyNodes", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.ApplicationHealthPolicies != null)
            {
                writer.WriteProperty(obj.ApplicationHealthPolicies, "ApplicationHealthPolicies", ApplicationHealthPoliciesConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
