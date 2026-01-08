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
    /// Converter for <see cref="ApplicationResourceUpgradeProgressInfo" />.
    /// </summary>
    internal class ApplicationResourceUpgradeProgressInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationResourceUpgradeProgressInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationResourceUpgradeProgressInfo GetFromJsonProperties(JsonReader reader)
        {
            var name = default(string);
            var targetApplicationTypeVersion = default(string);
            var startTimestampUtc = default(string);
            var upgradeState = default(ApplicationResourceUpgradeState?);
            var percentCompleted = default(string);
            var serviceUpgradeProgress = default(IEnumerable<ServiceUpgradeProgress>);
            var rollingUpgradeMode = default(RollingUpgradeMode?);
            var upgradeDuration = default(string);
            var applicationUpgradeStatusDetails = default(string);
            var upgradeReplicaSetCheckTimeoutInSeconds = default(long?);
            var failureTimestampUtc = default(string);

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
                else if (string.Compare("StartTimestampUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    startTimestampUtc = reader.ReadValueAsString();
                }
                else if (string.Compare("UpgradeState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeState = ApplicationResourceUpgradeStateConverter.Deserialize(reader);
                }
                else if (string.Compare("PercentCompleted", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    percentCompleted = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceUpgradeProgress", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceUpgradeProgress = reader.ReadList(ServiceUpgradeProgressConverter.Deserialize);
                }
                else if (string.Compare("RollingUpgradeMode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    rollingUpgradeMode = RollingUpgradeModeConverter.Deserialize(reader);
                }
                else if (string.Compare("UpgradeDuration", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeDuration = reader.ReadValueAsString();
                }
                else if (string.Compare("ApplicationUpgradeStatusDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationUpgradeStatusDetails = reader.ReadValueAsString();
                }
                else if (string.Compare("UpgradeReplicaSetCheckTimeoutInSeconds", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    upgradeReplicaSetCheckTimeoutInSeconds = reader.ReadValueAsLong();
                }
                else if (string.Compare("FailureTimestampUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    failureTimestampUtc = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationResourceUpgradeProgressInfo(
                name: name,
                targetApplicationTypeVersion: targetApplicationTypeVersion,
                startTimestampUtc: startTimestampUtc,
                upgradeState: upgradeState,
                percentCompleted: percentCompleted,
                serviceUpgradeProgress: serviceUpgradeProgress,
                rollingUpgradeMode: rollingUpgradeMode,
                upgradeDuration: upgradeDuration,
                applicationUpgradeStatusDetails: applicationUpgradeStatusDetails,
                upgradeReplicaSetCheckTimeoutInSeconds: upgradeReplicaSetCheckTimeoutInSeconds,
                failureTimestampUtc: failureTimestampUtc);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationResourceUpgradeProgressInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.UpgradeState, "UpgradeState", ApplicationResourceUpgradeStateConverter.Serialize);
            writer.WriteProperty(obj.RollingUpgradeMode, "RollingUpgradeMode", RollingUpgradeModeConverter.Serialize);
            if (obj.Name != null)
            {
                writer.WriteProperty(obj.Name, "Name", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.TargetApplicationTypeVersion != null)
            {
                writer.WriteProperty(obj.TargetApplicationTypeVersion, "TargetApplicationTypeVersion", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.StartTimestampUtc != null)
            {
                writer.WriteProperty(obj.StartTimestampUtc, "StartTimestampUtc", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.PercentCompleted != null)
            {
                writer.WriteProperty(obj.PercentCompleted, "PercentCompleted", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ServiceUpgradeProgress != null)
            {
                writer.WriteEnumerableProperty(obj.ServiceUpgradeProgress, "ServiceUpgradeProgress", ServiceUpgradeProgressConverter.Serialize);
            }

            if (obj.UpgradeDuration != null)
            {
                writer.WriteProperty(obj.UpgradeDuration, "UpgradeDuration", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ApplicationUpgradeStatusDetails != null)
            {
                writer.WriteProperty(obj.ApplicationUpgradeStatusDetails, "ApplicationUpgradeStatusDetails", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.UpgradeReplicaSetCheckTimeoutInSeconds != null)
            {
                writer.WriteProperty(obj.UpgradeReplicaSetCheckTimeoutInSeconds, "UpgradeReplicaSetCheckTimeoutInSeconds", JsonWriterExtensions.WriteLongValue);
            }

            if (obj.FailureTimestampUtc != null)
            {
                writer.WriteProperty(obj.FailureTimestampUtc, "FailureTimestampUtc", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
