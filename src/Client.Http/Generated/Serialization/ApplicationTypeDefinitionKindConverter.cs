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
    /// Converter for <see cref="ApplicationTypeDefinitionKind" />.
    /// </summary>
    internal class ApplicationTypeDefinitionKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ApplicationTypeDefinitionKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ApplicationTypeDefinitionKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationTypeDefinitionKind.Invalid;
            }
            else if (string.Compare(value, "ServiceFabricApplicationPackage", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationTypeDefinitionKind.ServiceFabricApplicationPackage;
            }
            else if (string.Compare(value, "Compose", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ApplicationTypeDefinitionKind.Compose;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ApplicationTypeDefinitionKind? value)
        {
            switch (value)
            {
                case ApplicationTypeDefinitionKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ApplicationTypeDefinitionKind.ServiceFabricApplicationPackage:
                    writer.WriteStringValue("ServiceFabricApplicationPackage");
                    break;
                case ApplicationTypeDefinitionKind.Compose:
                    writer.WriteStringValue("Compose");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ApplicationTypeDefinitionKind");
            }
        }
    }
}
