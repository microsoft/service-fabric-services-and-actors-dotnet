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
    /// Converter for <see cref="DeployedApplicationStatus" />.
    /// </summary>
    internal class DeployedApplicationStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static DeployedApplicationStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(DeployedApplicationStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeployedApplicationStatus.Invalid;
            }
            else if (string.Compare(value, "Downloading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeployedApplicationStatus.Downloading;
            }
            else if (string.Compare(value, "Activating", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeployedApplicationStatus.Activating;
            }
            else if (string.Compare(value, "Active", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeployedApplicationStatus.Active;
            }
            else if (string.Compare(value, "Upgrading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeployedApplicationStatus.Upgrading;
            }
            else if (string.Compare(value, "Deactivating", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeployedApplicationStatus.Deactivating;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, DeployedApplicationStatus? value)
        {
            switch (value)
            {
                case DeployedApplicationStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case DeployedApplicationStatus.Downloading:
                    writer.WriteStringValue("Downloading");
                    break;
                case DeployedApplicationStatus.Activating:
                    writer.WriteStringValue("Activating");
                    break;
                case DeployedApplicationStatus.Active:
                    writer.WriteStringValue("Active");
                    break;
                case DeployedApplicationStatus.Upgrading:
                    writer.WriteStringValue("Upgrading");
                    break;
                case DeployedApplicationStatus.Deactivating:
                    writer.WriteStringValue("Deactivating");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type DeployedApplicationStatus");
            }
        }
    }
}
