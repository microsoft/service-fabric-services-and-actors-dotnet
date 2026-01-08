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
    /// Converter for <see cref="ApplicationResourceUpgradeState" />.
    /// </summary>
    internal class ApplicationResourceUpgradeStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ApplicationResourceUpgradeState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ApplicationResourceUpgradeState);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.Invalid;
            }
            else if (string.Compare(value, "ProvisioningTarget", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.ProvisioningTarget;
            }
            else if (string.Compare(value, "RollingForward", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.RollingForward;
            }
            else if (string.Compare(value, "UnprovisioningCurrent", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.UnprovisioningCurrent;
            }
            else if (string.Compare(value, "CompletedRollforward", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.CompletedRollforward;
            }
            else if (string.Compare(value, "RollingBack", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.RollingBack;
            }
            else if (string.Compare(value, "UnprovisioningTarget", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.UnprovisioningTarget;
            }
            else if (string.Compare(value, "CompletedRollback", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.CompletedRollback;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationResourceUpgradeState.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ApplicationResourceUpgradeState? value)
        {
            switch (value)
            {
                case ApplicationResourceUpgradeState.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ApplicationResourceUpgradeState.ProvisioningTarget:
                    writer.WriteStringValue("ProvisioningTarget");
                    break;
                case ApplicationResourceUpgradeState.RollingForward:
                    writer.WriteStringValue("RollingForward");
                    break;
                case ApplicationResourceUpgradeState.UnprovisioningCurrent:
                    writer.WriteStringValue("UnprovisioningCurrent");
                    break;
                case ApplicationResourceUpgradeState.CompletedRollforward:
                    writer.WriteStringValue("CompletedRollforward");
                    break;
                case ApplicationResourceUpgradeState.RollingBack:
                    writer.WriteStringValue("RollingBack");
                    break;
                case ApplicationResourceUpgradeState.UnprovisioningTarget:
                    writer.WriteStringValue("UnprovisioningTarget");
                    break;
                case ApplicationResourceUpgradeState.CompletedRollback:
                    writer.WriteStringValue("CompletedRollback");
                    break;
                case ApplicationResourceUpgradeState.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ApplicationResourceUpgradeState");
            }
        }
    }
}
