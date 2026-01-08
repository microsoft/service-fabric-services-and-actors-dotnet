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
    /// Converter for <see cref="DeactivationIntent" />.
    /// </summary>
    internal class DeactivationIntentConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static DeactivationIntent? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(DeactivationIntent);

            if (string.Compare(value, "Pause", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeactivationIntent.Pause;
            }
            else if (string.Compare(value, "Restart", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeactivationIntent.Restart;
            }
            else if (string.Compare(value, "RemoveData", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeactivationIntent.RemoveData;
            }
            else if (string.Compare(value, "RemoveNode", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = DeactivationIntent.RemoveNode;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, DeactivationIntent? value)
        {
            switch (value)
            {
                case DeactivationIntent.Pause:
                    writer.WriteStringValue("Pause");
                    break;
                case DeactivationIntent.Restart:
                    writer.WriteStringValue("Restart");
                    break;
                case DeactivationIntent.RemoveData:
                    writer.WriteStringValue("RemoveData");
                    break;
                case DeactivationIntent.RemoveNode:
                    writer.WriteStringValue("RemoveNode");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type DeactivationIntent");
            }
        }
    }
}
