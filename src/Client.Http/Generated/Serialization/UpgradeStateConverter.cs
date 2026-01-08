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
    /// Converter for <see cref="UpgradeState" />.
    /// </summary>
    internal class UpgradeStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static UpgradeState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(UpgradeState);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeState.Invalid;
            }
            else if (string.Compare(value, "RollingBackInProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeState.RollingBackInProgress;
            }
            else if (string.Compare(value, "RollingBackCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeState.RollingBackCompleted;
            }
            else if (string.Compare(value, "RollingForwardPending", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeState.RollingForwardPending;
            }
            else if (string.Compare(value, "RollingForwardInProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeState.RollingForwardInProgress;
            }
            else if (string.Compare(value, "RollingForwardCompleted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeState.RollingForwardCompleted;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = UpgradeState.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, UpgradeState? value)
        {
            switch (value)
            {
                case UpgradeState.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case UpgradeState.RollingBackInProgress:
                    writer.WriteStringValue("RollingBackInProgress");
                    break;
                case UpgradeState.RollingBackCompleted:
                    writer.WriteStringValue("RollingBackCompleted");
                    break;
                case UpgradeState.RollingForwardPending:
                    writer.WriteStringValue("RollingForwardPending");
                    break;
                case UpgradeState.RollingForwardInProgress:
                    writer.WriteStringValue("RollingForwardInProgress");
                    break;
                case UpgradeState.RollingForwardCompleted:
                    writer.WriteStringValue("RollingForwardCompleted");
                    break;
                case UpgradeState.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type UpgradeState");
            }
        }
    }
}
