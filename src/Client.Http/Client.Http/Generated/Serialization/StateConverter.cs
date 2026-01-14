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
    /// Converter for <see cref="State" />.
    /// </summary>
    internal class StateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static State? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(State);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = State.Invalid;
            }
            else if (string.Compare(value, "Created", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = State.Created;
            }
            else if (string.Compare(value, "Claimed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = State.Claimed;
            }
            else if (string.Compare(value, "Preparing", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = State.Preparing;
            }
            else if (string.Compare(value, "Approved", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = State.Approved;
            }
            else if (string.Compare(value, "Executing", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = State.Executing;
            }
            else if (string.Compare(value, "Restoring", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = State.Restoring;
            }
            else if (string.Compare(value, "Completed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = State.Completed;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, State? value)
        {
            switch (value)
            {
                case State.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case State.Created:
                    writer.WriteStringValue("Created");
                    break;
                case State.Claimed:
                    writer.WriteStringValue("Claimed");
                    break;
                case State.Preparing:
                    writer.WriteStringValue("Preparing");
                    break;
                case State.Approved:
                    writer.WriteStringValue("Approved");
                    break;
                case State.Executing:
                    writer.WriteStringValue("Executing");
                    break;
                case State.Restoring:
                    writer.WriteStringValue("Restoring");
                    break;
                case State.Completed:
                    writer.WriteStringValue("Completed");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type State");
            }
        }
    }
}
