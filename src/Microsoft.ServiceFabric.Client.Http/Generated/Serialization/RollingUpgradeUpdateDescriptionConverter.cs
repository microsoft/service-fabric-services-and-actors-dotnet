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
    /// Converter for <see cref="RollingUpgradeUpdateDescription" />.
    /// </summary>
    internal class RollingUpgradeUpdateDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RollingUpgradeUpdateDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RollingUpgradeUpdateDescription GetFromJsonProperties(JsonReader reader)
        {
            var rollingUpgradeMode = default(UpgradeMode?);
            var forceRestart = default(bool?);
            var replicaSetCheckTimeoutInMilliseconds = default(long?);
            var failureAction = default(FailureAction?);
            var healthCheckWaitDurationInMilliseconds = default(string);
            var healthCheckStableDurationInMilliseconds = default(string);
            var healthCheckRetryTimeoutInMilliseconds = default(string);
            var upgradeTimeoutInMilliseconds = default(string);
            var upgradeDomainTimeoutInMilliseconds = default(string);
            var instanceCloseDelayDurationInSeconds = default(long?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("RollingUpgradeMode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    rollingUpgradeMode = UpgradeModeConverter.Deserialize(reader);
                }
                else if (string.Compare("ForceRestart", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    forceRestart = reader.ReadValueAsBool();
                }
                else if (string.Compare("ReplicaSetCheckTimeoutInMilliseconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    replicaSetCheckTimeoutInMilliseconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("FailureAction", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    failureAction = FailureActionConverter.Deserialize(reader);
                }
                else if (string.Compare("HealthCheckWaitDurationInMilliseconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthCheckWaitDurationInMilliseconds = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthCheckStableDurationInMilliseconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthCheckStableDurationInMilliseconds = reader.ReadValueAsString();
                }
                else if (string.Compare("HealthCheckRetryTimeoutInMilliseconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    healthCheckRetryTimeoutInMilliseconds = reader.ReadValueAsString();
                }
                else if (string.Compare("UpgradeTimeoutInMilliseconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeTimeoutInMilliseconds = reader.ReadValueAsString();
                }
                else if (string.Compare("UpgradeDomainTimeoutInMilliseconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeDomainTimeoutInMilliseconds = reader.ReadValueAsString();
                }
                else if (string.Compare("InstanceCloseDelayDurationInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceCloseDelayDurationInSeconds = reader.ReadValueAsLong();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RollingUpgradeUpdateDescription(
                rollingUpgradeMode: rollingUpgradeMode,
                forceRestart: forceRestart,
                replicaSetCheckTimeoutInMilliseconds: replicaSetCheckTimeoutInMilliseconds,
                failureAction: failureAction,
                healthCheckWaitDurationInMilliseconds: healthCheckWaitDurationInMilliseconds,
                healthCheckStableDurationInMilliseconds: healthCheckStableDurationInMilliseconds,
                healthCheckRetryTimeoutInMilliseconds: healthCheckRetryTimeoutInMilliseconds,
                upgradeTimeoutInMilliseconds: upgradeTimeoutInMilliseconds,
                upgradeDomainTimeoutInMilliseconds: upgradeDomainTimeoutInMilliseconds,
                instanceCloseDelayDurationInSeconds: instanceCloseDelayDurationInSeconds);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RollingUpgradeUpdateDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.RollingUpgradeMode, "RollingUpgradeMode", UpgradeModeConverter.Serialize);
            writer.WriteProperty(obj.FailureAction, "FailureAction", FailureActionConverter.Serialize);
            if (obj.ForceRestart != null)
            {
                writer.WriteProperty(obj.ForceRestart, "ForceRestart", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.ReplicaSetCheckTimeoutInMilliseconds != null)
            {
                writer.WriteProperty(obj.ReplicaSetCheckTimeoutInMilliseconds, "ReplicaSetCheckTimeoutInMilliseconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.HealthCheckWaitDurationInMilliseconds != null)
            {
                writer.WriteProperty(obj.HealthCheckWaitDurationInMilliseconds, "HealthCheckWaitDurationInMilliseconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HealthCheckStableDurationInMilliseconds != null)
            {
                writer.WriteProperty(obj.HealthCheckStableDurationInMilliseconds, "HealthCheckStableDurationInMilliseconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HealthCheckRetryTimeoutInMilliseconds != null)
            {
                writer.WriteProperty(obj.HealthCheckRetryTimeoutInMilliseconds, "HealthCheckRetryTimeoutInMilliseconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.UpgradeTimeoutInMilliseconds != null)
            {
                writer.WriteProperty(obj.UpgradeTimeoutInMilliseconds, "UpgradeTimeoutInMilliseconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.UpgradeDomainTimeoutInMilliseconds != null)
            {
                writer.WriteProperty(obj.UpgradeDomainTimeoutInMilliseconds, "UpgradeDomainTimeoutInMilliseconds", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.InstanceCloseDelayDurationInSeconds != null)
            {
                writer.WriteProperty(obj.InstanceCloseDelayDurationInSeconds, "InstanceCloseDelayDurationInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            writer.WriteEndObject();
        }
    }
}
