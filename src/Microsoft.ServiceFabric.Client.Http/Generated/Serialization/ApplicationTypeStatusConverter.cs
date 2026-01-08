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
    /// Converter for <see cref="ApplicationTypeStatus" />.
    /// </summary>
    internal class ApplicationTypeStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ApplicationTypeStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ApplicationTypeStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationTypeStatus.Invalid;
            }
            else if (string.Compare(value, "Provisioning", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationTypeStatus.Provisioning;
            }
            else if (string.Compare(value, "Available", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationTypeStatus.Available;
            }
            else if (string.Compare(value, "Unprovisioning", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationTypeStatus.Unprovisioning;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationTypeStatus.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ApplicationTypeStatus? value)
        {
            switch (value)
            {
                case ApplicationTypeStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ApplicationTypeStatus.Provisioning:
                    writer.WriteStringValue("Provisioning");
                    break;
                case ApplicationTypeStatus.Available:
                    writer.WriteStringValue("Available");
                    break;
                case ApplicationTypeStatus.Unprovisioning:
                    writer.WriteStringValue("Unprovisioning");
                    break;
                case ApplicationTypeStatus.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ApplicationTypeStatus");
            }
        }
    }
}
