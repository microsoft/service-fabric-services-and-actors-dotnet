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
    /// Converter for <see cref="ImpactLevel" />.
    /// </summary>
    internal class ImpactLevelConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ImpactLevel? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ImpactLevel);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactLevel.Invalid;
            }
            else if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactLevel.None;
            }
            else if (string.Compare(value, "Restart", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactLevel.Restart;
            }
            else if (string.Compare(value, "RemoveData", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactLevel.RemoveData;
            }
            else if (string.Compare(value, "RemoveNode", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ImpactLevel.RemoveNode;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ImpactLevel? value)
        {
            switch (value)
            {
                case ImpactLevel.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ImpactLevel.None:
                    writer.WriteStringValue("None");
                    break;
                case ImpactLevel.Restart:
                    writer.WriteStringValue("Restart");
                    break;
                case ImpactLevel.RemoveData:
                    writer.WriteStringValue("RemoveData");
                    break;
                case ImpactLevel.RemoveNode:
                    writer.WriteStringValue("RemoveNode");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ImpactLevel");
            }
        }
    }
}
