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
    /// Converter for <see cref="DeploymentStatus" />.
    /// </summary>
    internal class DeploymentStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static DeploymentStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(DeploymentStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeploymentStatus.Invalid;
            }
            else if (string.Compare(value, "Downloading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeploymentStatus.Downloading;
            }
            else if (string.Compare(value, "Activating", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeploymentStatus.Activating;
            }
            else if (string.Compare(value, "Active", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeploymentStatus.Active;
            }
            else if (string.Compare(value, "Upgrading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeploymentStatus.Upgrading;
            }
            else if (string.Compare(value, "Deactivating", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeploymentStatus.Deactivating;
            }
            else if (string.Compare(value, "RanToCompletion", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeploymentStatus.RanToCompletion;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeploymentStatus.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, DeploymentStatus? value)
        {
            switch (value)
            {
                case DeploymentStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case DeploymentStatus.Downloading:
                    writer.WriteStringValue("Downloading");
                    break;
                case DeploymentStatus.Activating:
                    writer.WriteStringValue("Activating");
                    break;
                case DeploymentStatus.Active:
                    writer.WriteStringValue("Active");
                    break;
                case DeploymentStatus.Upgrading:
                    writer.WriteStringValue("Upgrading");
                    break;
                case DeploymentStatus.Deactivating:
                    writer.WriteStringValue("Deactivating");
                    break;
                case DeploymentStatus.RanToCompletion:
                    writer.WriteStringValue("RanToCompletion");
                    break;
                case DeploymentStatus.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type DeploymentStatus");
            }
        }
    }
}
