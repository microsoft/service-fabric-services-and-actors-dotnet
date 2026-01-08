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
    /// Converter for <see cref="EntryPointStatus" />.
    /// </summary>
    internal class EntryPointStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static EntryPointStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(EntryPointStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntryPointStatus.Invalid;
            }
            else if (string.Compare(value, "Pending", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntryPointStatus.Pending;
            }
            else if (string.Compare(value, "Starting", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntryPointStatus.Starting;
            }
            else if (string.Compare(value, "Started", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntryPointStatus.Started;
            }
            else if (string.Compare(value, "Stopping", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntryPointStatus.Stopping;
            }
            else if (string.Compare(value, "Stopped", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = EntryPointStatus.Stopped;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, EntryPointStatus? value)
        {
            switch (value)
            {
                case EntryPointStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case EntryPointStatus.Pending:
                    writer.WriteStringValue("Pending");
                    break;
                case EntryPointStatus.Starting:
                    writer.WriteStringValue("Starting");
                    break;
                case EntryPointStatus.Started:
                    writer.WriteStringValue("Started");
                    break;
                case EntryPointStatus.Stopping:
                    writer.WriteStringValue("Stopping");
                    break;
                case EntryPointStatus.Stopped:
                    writer.WriteStringValue("Stopped");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type EntryPointStatus");
            }
        }
    }
}
