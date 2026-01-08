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
    /// Converter for <see cref="ComposeDeploymentUpgradeState" />.
    /// </summary>
    internal class ComposeDeploymentUpgradeStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ComposeDeploymentUpgradeState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ComposeDeploymentUpgradeState);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.Invalid;
            }
            else if (string.Compare(value, "ProvisioningTarget", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.ProvisioningTarget;
            }
            else if (string.Compare(value, "RollingForwardInProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.RollingForwardInProgress;
            }
            else if (string.Compare(value, "RollingForwardPending", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.RollingForwardPending;
            }
            else if (string.Compare(value, "UnprovisioningCurrent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.UnprovisioningCurrent;
            }
            else if (string.Compare(value, "RollingForwardCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.RollingForwardCompleted;
            }
            else if (string.Compare(value, "RollingBackInProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.RollingBackInProgress;
            }
            else if (string.Compare(value, "UnprovisioningTarget", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.UnprovisioningTarget;
            }
            else if (string.Compare(value, "RollingBackCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.RollingBackCompleted;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentUpgradeState.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ComposeDeploymentUpgradeState? value)
        {
            switch (value)
            {
                case ComposeDeploymentUpgradeState.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ComposeDeploymentUpgradeState.ProvisioningTarget:
                    writer.WriteStringValue("ProvisioningTarget");
                    break;
                case ComposeDeploymentUpgradeState.RollingForwardInProgress:
                    writer.WriteStringValue("RollingForwardInProgress");
                    break;
                case ComposeDeploymentUpgradeState.RollingForwardPending:
                    writer.WriteStringValue("RollingForwardPending");
                    break;
                case ComposeDeploymentUpgradeState.UnprovisioningCurrent:
                    writer.WriteStringValue("UnprovisioningCurrent");
                    break;
                case ComposeDeploymentUpgradeState.RollingForwardCompleted:
                    writer.WriteStringValue("RollingForwardCompleted");
                    break;
                case ComposeDeploymentUpgradeState.RollingBackInProgress:
                    writer.WriteStringValue("RollingBackInProgress");
                    break;
                case ComposeDeploymentUpgradeState.UnprovisioningTarget:
                    writer.WriteStringValue("UnprovisioningTarget");
                    break;
                case ComposeDeploymentUpgradeState.RollingBackCompleted:
                    writer.WriteStringValue("RollingBackCompleted");
                    break;
                case ComposeDeploymentUpgradeState.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ComposeDeploymentUpgradeState");
            }
        }
    }
}
