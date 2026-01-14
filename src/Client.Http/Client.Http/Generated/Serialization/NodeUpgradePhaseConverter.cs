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
    /// Converter for <see cref="NodeUpgradePhase" />.
    /// </summary>
    internal class NodeUpgradePhaseConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static NodeUpgradePhase? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(NodeUpgradePhase);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeUpgradePhase.Invalid;
            }
            else if (string.Compare(value, "PreUpgradeSafetyCheck", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeUpgradePhase.PreUpgradeSafetyCheck;
            }
            else if (string.Compare(value, "Upgrading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeUpgradePhase.Upgrading;
            }
            else if (string.Compare(value, "PostUpgradeSafetyCheck", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = NodeUpgradePhase.PostUpgradeSafetyCheck;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, NodeUpgradePhase? value)
        {
            switch (value)
            {
                case NodeUpgradePhase.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case NodeUpgradePhase.PreUpgradeSafetyCheck:
                    writer.WriteStringValue("PreUpgradeSafetyCheck");
                    break;
                case NodeUpgradePhase.Upgrading:
                    writer.WriteStringValue("Upgrading");
                    break;
                case NodeUpgradePhase.PostUpgradeSafetyCheck:
                    writer.WriteStringValue("PostUpgradeSafetyCheck");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type NodeUpgradePhase");
            }
        }
    }
}
