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
    /// Converter for <see cref="ServiceEndpointRole" />.
    /// </summary>
    internal class ServiceEndpointRoleConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServiceEndpointRole? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServiceEndpointRole);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceEndpointRole.Invalid;
            }
            else if (string.Compare(value, "Stateless", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceEndpointRole.Stateless;
            }
            else if (string.Compare(value, "StatefulPrimary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceEndpointRole.StatefulPrimary;
            }
            else if (string.Compare(value, "StatefulSecondary", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceEndpointRole.StatefulSecondary;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServiceEndpointRole? value)
        {
            switch (value)
            {
                case ServiceEndpointRole.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ServiceEndpointRole.Stateless:
                    writer.WriteStringValue("Stateless");
                    break;
                case ServiceEndpointRole.StatefulPrimary:
                    writer.WriteStringValue("StatefulPrimary");
                    break;
                case ServiceEndpointRole.StatefulSecondary:
                    writer.WriteStringValue("StatefulSecondary");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServiceEndpointRole");
            }
        }
    }
}
