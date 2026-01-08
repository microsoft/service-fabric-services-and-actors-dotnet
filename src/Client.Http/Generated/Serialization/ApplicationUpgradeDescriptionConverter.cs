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
    /// Converter for <see cref="ApplicationUpgradeDescription" />.
    /// </summary>
    internal class ApplicationUpgradeDescriptionConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationUpgradeDescription Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationUpgradeDescription GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var targetApplicationTypeVersion = default(string);
            var parameters = default(IReadOnlyDictionary<string, string>);
            var upgradeKind = default(UpgradeKind?);
            var rollingUpgradeMode = default(UpgradeMode?);
            var upgradeReplicaSetCheckTimeoutInSeconds = default(long?);
            var forceRestart = default(bool?);
            var sortOrder = default(UpgradeSortOrder?);
            var monitoringPolicy = default(MonitoringPolicyDescription);
            var applicationHealthPolicy = default(ApplicationHealthPolicy);
            var instanceCloseDelayDurationInSeconds = default(long?);
            var managedApplicationIdentity = default(ManagedApplicationIdentityDescription);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("Name", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    name = reader.ReadValueAsString();
                }
                else if (string.Compare("TargetApplicationTypeVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    targetApplicationTypeVersion = reader.ReadValueAsString();
                }
                else if (string.Compare("Parameters", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    parameters = ApplicationParametersConverter.Deserialize(reader);
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
                else if (string.Compare("SortOrder", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sortOrder = UpgradeSortOrderConverter.Deserialize(reader);
                }
                else if (string.Compare("MonitoringPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    monitoringPolicy = MonitoringPolicyDescriptionConverter.Deserialize(reader);
                }
                else if (string.Compare("ApplicationHealthPolicy", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationHealthPolicy = ApplicationHealthPolicyConverter.Deserialize(reader);
                }
                else if (string.Compare("InstanceCloseDelayDurationInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    instanceCloseDelayDurationInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("ManagedApplicationIdentity", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    managedApplicationIdentity = ManagedApplicationIdentityDescriptionConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationUpgradeDescription(
                name: name,
                targetApplicationTypeVersion: targetApplicationTypeVersion,
                parameters: parameters,
                upgradeKind: upgradeKind,
                rollingUpgradeMode: rollingUpgradeMode,
                upgradeReplicaSetCheckTimeoutInSeconds: upgradeReplicaSetCheckTimeoutInSeconds,
                forceRestart: forceRestart,
                sortOrder: sortOrder,
                monitoringPolicy: monitoringPolicy,
                applicationHealthPolicy: applicationHealthPolicy,
                instanceCloseDelayDurationInSeconds: instanceCloseDelayDurationInSeconds,
                managedApplicationIdentity: managedApplicationIdentity);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationUpgradeDescription obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UpgradeKind, "UpgradeKind", UpgradeKindConverter.Serialize);
            writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.TargetApplicationTypeVersion, "TargetApplicationTypeVersion", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.RollingUpgradeMode, "RollingUpgradeMode", UpgradeModeConverter.Serialize);
            writer.WriteProperty(obj.SortOrder, "SortOrder", UpgradeSortOrderConverter.Serialize);
            if (obj.Parameters != null)
            {
                writer.WriteProperty(obj.Parameters, "Parameters", ApplicationParametersConverter.Serialize);
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

            if (obj.InstanceCloseDelayDurationInSeconds != null)
            {
                writer.WriteProperty(obj.InstanceCloseDelayDurationInSeconds, "InstanceCloseDelayDurationInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.ManagedApplicationIdentity != null)
            {
                writer.WriteProperty(obj.ManagedApplicationIdentity, "ManagedApplicationIdentity", ManagedApplicationIdentityDescriptionConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
