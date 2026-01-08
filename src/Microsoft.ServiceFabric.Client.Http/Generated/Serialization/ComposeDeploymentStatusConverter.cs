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
    /// Converter for <see cref="ComposeDeploymentStatus" />.
    /// </summary>
    internal class ComposeDeploymentStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ComposeDeploymentStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ComposeDeploymentStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentStatus.Invalid;
            }
            else if (string.Compare(value, "Provisioning", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentStatus.Provisioning;
            }
            else if (string.Compare(value, "Creating", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentStatus.Creating;
            }
            else if (string.Compare(value, "Ready", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentStatus.Ready;
            }
            else if (string.Compare(value, "Unprovisioning", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentStatus.Unprovisioning;
            }
            else if (string.Compare(value, "Deleting", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentStatus.Deleting;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentStatus.Failed;
            }
            else if (string.Compare(value, "Upgrading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ComposeDeploymentStatus.Upgrading;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ComposeDeploymentStatus? value)
        {
            switch (value)
            {
                case ComposeDeploymentStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ComposeDeploymentStatus.Provisioning:
                    writer.WriteStringValue("Provisioning");
                    break;
                case ComposeDeploymentStatus.Creating:
                    writer.WriteStringValue("Creating");
                    break;
                case ComposeDeploymentStatus.Ready:
                    writer.WriteStringValue("Ready");
                    break;
                case ComposeDeploymentStatus.Unprovisioning:
                    writer.WriteStringValue("Unprovisioning");
                    break;
                case ComposeDeploymentStatus.Deleting:
                    writer.WriteStringValue("Deleting");
                    break;
                case ComposeDeploymentStatus.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                case ComposeDeploymentStatus.Upgrading:
                    writer.WriteStringValue("Upgrading");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ComposeDeploymentStatus");
            }
        }
    }
}
