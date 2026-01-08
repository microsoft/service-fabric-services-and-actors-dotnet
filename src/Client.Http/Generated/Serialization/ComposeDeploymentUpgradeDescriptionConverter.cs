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
    /// Converter for <see cref="ComposeDeploymentUpgradeDescription" />.
    /// </summary>
    internal class ComposeDeploymentUpgradeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ComposeDeploymentUpgradeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ComposeDeploymentUpgradeDescription GetFromJsonProperties(JsonReader reader)
        {
            var deploymentName = default(string);
            var composeFileContent = default(string);
            var registryCredential = default(RegistryCredential);
            var upgradeKind = default(UpgradeKind?);
            var rollingUpgradeMode = default(UpgradeMode?);
            var upgradeReplicaSetCheckTimeoutInSeconds = default(long?);
            var forceRestart = default(bool?);
            var monitoringPolicy = default(MonitoringPolicyDescription);
            var applicationHealthPolicy = default(ApplicationHealthPolicy);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("DeploymentName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    deploymentName = reader.ReadValueAsString();
                }
                else if (string.Compare("ComposeFileContent", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    composeFileContent = reader.ReadValueAsString();
                }
                else if (string.Compare("RegistryCredential", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    registryCredential = RegistryCredentialConverter.Deserialize(reader);
                }
                else if (string.Compare("UpgradeKind", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeKind = UpgradeKindConverter.Deserialize(reader);
                }
                else if (string.Compare("RollingUpgradeMode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    rollingUpgradeMode = UpgradeModeConverter.Deserialize(reader);
                }
                else if (string.Compare("UpgradeReplicaSetCheckTimeoutInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeReplicaSetCheckTimeoutInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("ForceRestart", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    forceRestart = reader.ReadValueAsBool();
                }
                else if (string.Compare("MonitoringPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    monitoringPolicy = MonitoringPolicyDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthPolicy = ApplicationHealthPolicyConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ComposeDeploymentUpgradeDescription(
                deploymentName: deploymentName,
                composeFileContent: composeFileContent,
                registryCredential: registryCredential,
                upgradeKind: upgradeKind,
                rollingUpgradeMode: rollingUpgradeMode,
                upgradeReplicaSetCheckTimeoutInSeconds: upgradeReplicaSetCheckTimeoutInSeconds,
                forceRestart: forceRestart,
                monitoringPolicy: monitoringPolicy,
                applicationHealthPolicy: applicationHealthPolicy);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ComposeDeploymentUpgradeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UpgradeKind, "UpgradeKind", UpgradeKindConverter.Serialize);
            writer.WriteProperty(obj.DeploymentName, "DeploymentName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ComposeFileContent, "ComposeFileContent", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.RollingUpgradeMode, "RollingUpgradeMode", UpgradeModeConverter.Serialize);
            if (obj.RegistryCredential != null)
            {
                writer.WriteProperty(obj.RegistryCredential, "RegistryCredential", RegistryCredentialConverter.Serialize);
            }

            if (obj.UpgradeReplicaSetCheckTimeoutInSeconds != null)
            {
                writer.WriteProperty(obj.UpgradeReplicaSetCheckTimeoutInSeconds, "UpgradeReplicaSetCheckTimeoutInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ForceRestart != null)
            {
                writer.WriteProperty(obj.ForceRestart, "ForceRestart", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.MonitoringPolicy != null)
            {
                writer.WriteProperty(obj.MonitoringPolicy, "MonitoringPolicy", MonitoringPolicyDescriptionConverter.Serialize);
            }

            if (obj.ApplicationHealthPolicy != null)
            {
                writer.WriteProperty(obj.ApplicationHealthPolicy, "ApplicationHealthPolicy", ApplicationHealthPolicyConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
