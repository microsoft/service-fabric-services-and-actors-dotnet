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
    /// Converter for <see cref="ResourceStatus" />.
    /// </summary>
    internal class ResourceStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ResourceStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ResourceStatus);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResourceStatus.Unknown;
            }
            else if (string.Compare(value, "Ready", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResourceStatus.Ready;
            }
            else if (string.Compare(value, "Upgrading", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResourceStatus.Upgrading;
            }
            else if (string.Compare(value, "Creating", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResourceStatus.Creating;
            }
            else if (string.Compare(value, "Deleting", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResourceStatus.Deleting;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResourceStatus.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ResourceStatus? value)
        {
            switch (value)
            {
                case ResourceStatus.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case ResourceStatus.Ready:
                    writer.WriteStringValue("Ready");
                    break;
                case ResourceStatus.Upgrading:
                    writer.WriteStringValue("Upgrading");
                    break;
                case ResourceStatus.Creating:
                    writer.WriteStringValue("Creating");
                    break;
                case ResourceStatus.Deleting:
                    writer.WriteStringValue("Deleting");
                    break;
                case ResourceStatus.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ResourceStatus");
            }
        }
    }
}
