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
    /// Converter for <see cref="EnvironmentVariableType" />.
    /// </summary>
    internal class EnvironmentVariableTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static EnvironmentVariableType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(EnvironmentVariableType);

            if (string.Compare(value, "ClearText", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EnvironmentVariableType.ClearText;
            }
            else if (string.Compare(value, "KeyVaultReference", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EnvironmentVariableType.KeyVaultReference;
            }
            else if (string.Compare(value, "SecretValueReference", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EnvironmentVariableType.SecretValueReference;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, EnvironmentVariableType? value)
        {
            switch (value)
            {
                case EnvironmentVariableType.ClearText:
                    writer.WriteStringValue("ClearText");
                    break;
                case EnvironmentVariableType.KeyVaultReference:
                    writer.WriteStringValue("KeyVaultReference");
                    break;
                case EnvironmentVariableType.SecretValueReference:
                    writer.WriteStringValue("SecretValueReference");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type EnvironmentVariableType");
            }
        }
    }
}
