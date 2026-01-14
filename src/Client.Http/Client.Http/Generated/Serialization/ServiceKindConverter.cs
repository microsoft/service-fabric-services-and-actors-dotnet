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
    /// Converter for <see cref="ServiceKind" />.
    /// </summary>
    internal class ServiceKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServiceKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServiceKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceKind.Invalid;
            }
            else if (string.Compare(value, "Stateless", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceKind.Stateless;
            }
            else if (string.Compare(value, "Stateful", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceKind.Stateful;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServiceKind? value)
        {
            switch (value)
            {
                case ServiceKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ServiceKind.Stateless:
                    writer.WriteStringValue("Stateless");
                    break;
                case ServiceKind.Stateful:
                    writer.WriteStringValue("Stateful");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServiceKind");
            }
        }
    }
}
