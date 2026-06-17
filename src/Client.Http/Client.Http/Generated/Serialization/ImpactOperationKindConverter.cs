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
    /// Converter for <see cref="ImpactOperationKind" />.
    /// </summary>
    internal class ImpactOperationKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ImpactOperationKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ImpactOperationKind);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactOperationKind.Unknown;
            }
            else if (string.Compare(value, "Restart", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactOperationKind.Restart;
            }
            else if (string.Compare(value, "Remove", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactOperationKind.Remove;
            }
            else if (string.Compare(value, "Add", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactOperationKind.Add;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ImpactOperationKind? value)
        {
            switch (value)
            {
                case ImpactOperationKind.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case ImpactOperationKind.Restart:
                    writer.WriteStringValue("Restart");
                    break;
                case ImpactOperationKind.Remove:
                    writer.WriteStringValue("Remove");
                    break;
                case ImpactOperationKind.Add:
                    writer.WriteStringValue("Add");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ImpactOperationKind");
            }
        }
    }
}
