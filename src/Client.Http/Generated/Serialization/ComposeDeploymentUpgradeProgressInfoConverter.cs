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
    /// Converter for <see cref="ComposeDeploymentUpgradeProgressInfo" />.
    /// </summary>
    internal class ComposeDeploymentUpgradeProgressInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ComposeDeploymentUpgradeProgressInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ComposeDeploymentUpgradeProgressInfo GetFromJsonProperties(JsonReader reader)
        {
            var deploymentName = default(string);
            var applicationName = default(string);
            var upgradeState = default(ComposeDeploymentUpgradeState?);
            var upgradeStatusDetails = default(string);
            var upgradeKind = default(UpgradeKind?);
            var rollingUpgradeMode = default(UpgradeMode?);
            var forceRestart = default(bool?);
            var upgradeReplicaSetCheckTimeoutInSeconds = default(long?);
            var monitoringPolicy = default(MonitoringPolicyDescription);
            var applicationHealthPolicy = default(ApplicationHealthPolicy);
            var targetApplicationTypeVersion = default(string);
            var upgradeDuration = default(string);
            var currentUpgradeDomainDuration = default(string);
            var applicationUnhealthyEvaluations = default(IEnumerable<HealthEvaluationWrapper>);
            var currentUpgradeDomainProgress = default(CurrentUpgradeDomainProgressInfo);
            var startTimestampUtc = default(string);
            var failureTimestampUtc = default(string);
            var failureReason = default(FailureReason?);
            var upgradeDomainProgressAtFailure = default(FailureUpgradeDomainProgressInfo);
            var applicationUpgradeStatusDetails = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("DeploymentName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deploymentName = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationName = reader.ReadValueAsString();
                }
                else if (string.Compare("UpgradeState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeState = ComposeDeploymentUpgradeStateConverter.Deserialize(reader);
                }
                else if (string.Compare("UpgradeStatusDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeStatusDetails = reader.ReadValueAsString();
                }
                else if (string.Compare("UpgradeKind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeKind = UpgradeKindConverter.Deserialize(reader);
                }
                else if (string.Compare("RollingUpgradeMode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    rollingUpgradeMode = UpgradeModeConverter.Deserialize(reader);
                }
                else if (string.Compare("ForceRestart", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    forceRestart = reader.ReadValueAsBool();
                }
                else if (string.Compare("UpgradeReplicaSetCheckTimeoutInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeReplicaSetCheckTimeoutInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("MonitoringPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    monitoringPolicy = MonitoringPolicyDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthPolicy = ApplicationHealthPolicyConverter.Deserialize(reader);
                }
                else if (string.Compare("TargetApplicationTypeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    targetApplicationTypeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("UpgradeDuration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeDuration = reader.ReadValueAsString();
                }
                else if (string.Compare("CurrentUpgradeDomainDuration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentUpgradeDomainDuration = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationUnhealthyEvaluations", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationUnhealthyEvaluations = reader.ReadList(HealthEvaluationWrapperConverter.Deserialize);
                }
                else if (string.Compare("CurrentUpgradeDomainProgress", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentUpgradeDomainProgress = CurrentUpgradeDomainProgressInfoConverter.Deserialize(reader);
                }
                else if (string.Compare("StartTimestampUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    startTimestampUtc = reader.ReadValueAsString();
                }
                else if (string.Compare("FailureTimestampUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    failureTimestampUtc = reader.ReadValueAsString();
                }
                else if (string.Compare("FailureReason", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    failureReason = FailureReasonConverter.Deserialize(reader);
                }
                else if (string.Compare("UpgradeDomainProgressAtFailure", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeDomainProgressAtFailure = FailureUpgradeDomainProgressInfoConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationUpgradeStatusDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationUpgradeStatusDetails = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ComposeDeploymentUpgradeProgressInfo(
                deploymentName: deploymentName,
                applicationName: applicationName,
                upgradeState: upgradeState,
                upgradeStatusDetails: upgradeStatusDetails,
                upgradeKind: upgradeKind,
                rollingUpgradeMode: rollingUpgradeMode,
                forceRestart: forceRestart,
                upgradeReplicaSetCheckTimeoutInSeconds: upgradeReplicaSetCheckTimeoutInSeconds,
                monitoringPolicy: monitoringPolicy,
                applicationHealthPolicy: applicationHealthPolicy,
                targetApplicationTypeVersion: targetApplicationTypeVersion,
                upgradeDuration: upgradeDuration,
                currentUpgradeDomainDuration: currentUpgradeDomainDuration,
                applicationUnhealthyEvaluations: applicationUnhealthyEvaluations,
                currentUpgradeDomainProgress: currentUpgradeDomainProgress,
                startTimestampUtc: startTimestampUtc,
                failureTimestampUtc: failureTimestampUtc,
                failureReason: failureReason,
                upgradeDomainProgressAtFailure: upgradeDomainProgressAtFailure,
                applicationUpgradeStatusDetails: applicationUpgradeStatusDetails);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ComposeDeploymentUpgradeProgressInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UpgradeState, "UpgradeState", ComposeDeploymentUpgradeStateConverter.Serialize);
            writer.WriteProperty(obj.UpgradeKind, "UpgradeKind", UpgradeKindConverter.Serialize);
            writer.WriteProperty(obj.RollingUpgradeMode, "RollingUpgradeMode", UpgradeModeConverter.Serialize);
            writer.WriteProperty(obj.FailureReason, "FailureReason", FailureReasonConverter.Serialize);
            if (obj.DeploymentName != null)
            {
                writer.WriteProperty(obj.DeploymentName, "DeploymentName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ApplicationName != null)
            {
                writer.WriteProperty(obj.ApplicationName, "ApplicationName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.UpgradeStatusDetails != null)
            {
                writer.WriteProperty(obj.UpgradeStatusDetails, "UpgradeStatusDetails", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ForceRestart != null)
            {
                writer.WriteProperty(obj.ForceRestart, "ForceRestart", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.UpgradeReplicaSetCheckTimeoutInSeconds != null)
            {
                writer.WriteProperty(obj.UpgradeReplicaSetCheckTimeoutInSeconds, "UpgradeReplicaSetCheckTimeoutInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.MonitoringPolicy != null)
            {
                writer.WriteProperty(obj.MonitoringPolicy, "MonitoringPolicy", MonitoringPolicyDescriptionConverter.Serialize);
            }

            if (obj.ApplicationHealthPolicy != null)
            {
                writer.WriteProperty(obj.ApplicationHealthPolicy, "ApplicationHealthPolicy", ApplicationHealthPolicyConverter.Serialize);
            }

            if (obj.TargetApplicationTypeVersion != null)
            {
                writer.WriteProperty(obj.TargetApplicationTypeVersion, "TargetApplicationTypeVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.UpgradeDuration != null)
            {
                writer.WriteProperty(obj.UpgradeDuration, "UpgradeDuration", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.CurrentUpgradeDomainDuration != null)
            {
                writer.WriteProperty(obj.CurrentUpgradeDomainDuration, "CurrentUpgradeDomainDuration", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ApplicationUnhealthyEvaluations != null)
            {
                writer.WriteEnumerableProperty(obj.ApplicationUnhealthyEvaluations, "ApplicationUnhealthyEvaluations", HealthEvaluationWrapperConverter.Serialize);
            }

            if (obj.CurrentUpgradeDomainProgress != null)
            {
                writer.WriteProperty(obj.CurrentUpgradeDomainProgress, "CurrentUpgradeDomainProgress", CurrentUpgradeDomainProgressInfoConverter.Serialize);
            }

            if (obj.StartTimestampUtc != null)
            {
                writer.WriteProperty(obj.StartTimestampUtc, "StartTimestampUtc", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FailureTimestampUtc != null)
            {
                writer.WriteProperty(obj.FailureTimestampUtc, "FailureTimestampUtc", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.UpgradeDomainProgressAtFailure != null)
            {
                writer.WriteProperty(obj.UpgradeDomainProgressAtFailure, "UpgradeDomainProgressAtFailure", FailureUpgradeDomainProgressInfoConverter.Serialize);
            }

            if (obj.ApplicationUpgradeStatusDetails != null)
            {
                writer.WriteProperty(obj.ApplicationUpgradeStatusDetails, "ApplicationUpgradeStatusDetails", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
