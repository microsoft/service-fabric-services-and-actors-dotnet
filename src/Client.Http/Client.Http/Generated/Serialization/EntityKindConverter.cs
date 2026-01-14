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
    /// Converter for <see cref="EntityKind" />.
    /// </summary>
    internal class EntityKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static EntityKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(EntityKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.Invalid;
            }
            else if (string.Compare(value, "Node", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.Node;
            }
            else if (string.Compare(value, "Partition", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.Partition;
            }
            else if (string.Compare(value, "Service", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.Service;
            }
            else if (string.Compare(value, "Application", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.Application;
            }
            else if (string.Compare(value, "Replica", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.Replica;
            }
            else if (string.Compare(value, "DeployedApplication", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.DeployedApplication;
            }
            else if (string.Compare(value, "DeployedServicePackage", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.DeployedServicePackage;
            }
            else if (string.Compare(value, "Cluster", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntityKind.Cluster;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, EntityKind? value)
        {
            switch (value)
            {
                case EntityKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case EntityKind.Node:
                    writer.WriteStringValue("Node");
                    break;
                case EntityKind.Partition:
                    writer.WriteStringValue("Partition");
                    break;
                case EntityKind.Service:
                    writer.WriteStringValue("Service");
                    break;
                case EntityKind.Application:
                    writer.WriteStringValue("Application");
                    break;
                case EntityKind.Replica:
                    writer.WriteStringValue("Replica");
                    break;
                case EntityKind.DeployedApplication:
                    writer.WriteStringValue("DeployedApplication");
                    break;
                case EntityKind.DeployedServicePackage:
                    writer.WriteStringValue("DeployedServicePackage");
                    break;
                case EntityKind.Cluster:
                    writer.WriteStringValue("Cluster");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type EntityKind");
            }
        }
    }
}
