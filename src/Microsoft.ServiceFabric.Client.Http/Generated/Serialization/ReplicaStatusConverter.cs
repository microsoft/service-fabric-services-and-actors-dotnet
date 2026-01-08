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
    /// Converter for <see cref="ReplicaStatus" />.
    /// </summary>
    internal class ReplicaStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ReplicaStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ReplicaStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicaStatus.Invalid;
            }
            else if (string.Compare(value, "InBuild", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicaStatus.InBuild;
            }
            else if (string.Compare(value, "Standby", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicaStatus.Standby;
            }
            else if (string.Compare(value, "Ready", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicaStatus.Ready;
            }
            else if (string.Compare(value, "Down", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicaStatus.Down;
            }
            else if (string.Compare(value, "Dropped", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicaStatus.Dropped;
            }
            else if (string.Compare(value, "Completed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicaStatus.Completed;
            }
            else if (string.Compare(value, "ToBeRemoved", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicaStatus.ToBeRemoved;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ReplicaStatus? value)
        {
            switch (value)
            {
                case ReplicaStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ReplicaStatus.InBuild:
                    writer.WriteStringValue("InBuild");
                    break;
                case ReplicaStatus.Standby:
                    writer.WriteStringValue("Standby");
                    break;
                case ReplicaStatus.Ready:
                    writer.WriteStringValue("Ready");
                    break;
                case ReplicaStatus.Down:
                    writer.WriteStringValue("Down");
                    break;
                case ReplicaStatus.Dropped:
                    writer.WriteStringValue("Dropped");
                    break;
                case ReplicaStatus.Completed:
                    writer.WriteStringValue("Completed");
                    break;
                case ReplicaStatus.ToBeRemoved:
                    writer.WriteStringValue("ToBeRemoved");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ReplicaStatus");
            }
        }
    }
}
