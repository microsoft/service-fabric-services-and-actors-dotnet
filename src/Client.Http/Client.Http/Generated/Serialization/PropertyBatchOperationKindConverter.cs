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
    /// Converter for <see cref="PropertyBatchOperationKind" />.
    /// </summary>
    internal class PropertyBatchOperationKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static PropertyBatchOperationKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(PropertyBatchOperationKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchOperationKind.Invalid;
            }
            else if (string.Compare(value, "Put", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchOperationKind.Put;
            }
            else if (string.Compare(value, "Get", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchOperationKind.Get;
            }
            else if (string.Compare(value, "CheckExists", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchOperationKind.CheckExists;
            }
            else if (string.Compare(value, "CheckSequence", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchOperationKind.CheckSequence;
            }
            else if (string.Compare(value, "Delete", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchOperationKind.Delete;
            }
            else if (string.Compare(value, "CheckValue", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = PropertyBatchOperationKind.CheckValue;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, PropertyBatchOperationKind? value)
        {
            switch (value)
            {
                case PropertyBatchOperationKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case PropertyBatchOperationKind.Put:
                    writer.WriteStringValue("Put");
                    break;
                case PropertyBatchOperationKind.Get:
                    writer.WriteStringValue("Get");
                    break;
                case PropertyBatchOperationKind.CheckExists:
                    writer.WriteStringValue("CheckExists");
                    break;
                case PropertyBatchOperationKind.CheckSequence:
                    writer.WriteStringValue("CheckSequence");
                    break;
                case PropertyBatchOperationKind.Delete:
                    writer.WriteStringValue("Delete");
                    break;
                case PropertyBatchOperationKind.CheckValue:
                    writer.WriteStringValue("CheckValue");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type PropertyBatchOperationKind");
            }
        }
    }
}
