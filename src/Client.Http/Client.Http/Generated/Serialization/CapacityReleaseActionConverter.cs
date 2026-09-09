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
    /// Converter for <see cref="CapacityReleaseAction" />.
    /// </summary>
    internal class CapacityReleaseActionConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static CapacityReleaseAction? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(CapacityReleaseAction);

            if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CapacityReleaseAction.None;
            }
            else if (string.Compare(value, "DropToZero", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CapacityReleaseAction.DropToZero;
            }
            else if (string.Compare(value, "DropToMin", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CapacityReleaseAction.DropToMin;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, CapacityReleaseAction? value)
        {
            switch (value)
            {
                case CapacityReleaseAction.None:
                    writer.WriteStringValue("None");
                    break;
                case CapacityReleaseAction.DropToZero:
                    writer.WriteStringValue("DropToZero");
                    break;
                case CapacityReleaseAction.DropToMin:
                    writer.WriteStringValue("DropToMin");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type CapacityReleaseAction");
            }
        }
    }
}
