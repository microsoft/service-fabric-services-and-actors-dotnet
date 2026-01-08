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
    /// Converter for <see cref="ProvisionApplicationTypeKind" />.
    /// </summary>
    internal class ProvisionApplicationTypeKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ProvisionApplicationTypeKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ProvisionApplicationTypeKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ProvisionApplicationTypeKind.Invalid;
            }
            else if (string.Compare(value, "ImageStorePath", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ProvisionApplicationTypeKind.ImageStorePath;
            }
            else if (string.Compare(value, "ExternalStore", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ProvisionApplicationTypeKind.ExternalStore;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ProvisionApplicationTypeKind? value)
        {
            switch (value)
            {
                case ProvisionApplicationTypeKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ProvisionApplicationTypeKind.ImageStorePath:
                    writer.WriteStringValue("ImageStorePath");
                    break;
                case ProvisionApplicationTypeKind.ExternalStore:
                    writer.WriteStringValue("ExternalStore");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ProvisionApplicationTypeKind");
            }
        }
    }
}
