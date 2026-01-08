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
    /// Converter for <see cref="PropertyBatchInfoKind" />.
    /// </summary>
    internal class PropertyBatchInfoKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static PropertyBatchInfoKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(PropertyBatchInfoKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchInfoKind.Invalid;
            }
            else if (string.Compare(value, "Successful", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchInfoKind.Successful;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchInfoKind.Failed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, PropertyBatchInfoKind? value)
        {
            switch (value)
            {
                case PropertyBatchInfoKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case PropertyBatchInfoKind.Successful:
                    writer.WriteStringValue("Successful");
                    break;
                case PropertyBatchInfoKind.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type PropertyBatchInfoKind");
            }
        }
    }
}
