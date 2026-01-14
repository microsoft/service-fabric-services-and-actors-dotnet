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
    /// Converter for <see cref="ServiceStatus" />.
    /// </summary>
    internal class ServiceStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServiceStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServiceStatus);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceStatus.Unknown;
            }
            else if (string.Compare(value, "Active", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceStatus.Active;
            }
            else if (string.Compare(value, "Upgrading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceStatus.Upgrading;
            }
            else if (string.Compare(value, "Deleting", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceStatus.Deleting;
            }
            else if (string.Compare(value, "Creating", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceStatus.Creating;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceStatus.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServiceStatus? value)
        {
            switch (value)
            {
                case ServiceStatus.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case ServiceStatus.Active:
                    writer.WriteStringValue("Active");
                    break;
                case ServiceStatus.Upgrading:
                    writer.WriteStringValue("Upgrading");
                    break;
                case ServiceStatus.Deleting:
                    writer.WriteStringValue("Deleting");
                    break;
                case ServiceStatus.Creating:
                    writer.WriteStringValue("Creating");
                    break;
                case ServiceStatus.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServiceStatus");
            }
        }
    }
}
