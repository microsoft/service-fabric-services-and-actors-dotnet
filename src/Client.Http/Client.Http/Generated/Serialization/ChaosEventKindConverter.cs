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
    /// Converter for <see cref="ChaosEventKind" />.
    /// </summary>
    internal class ChaosEventKindConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ChaosEventKind? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ChaosEventKind);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosEventKind.Invalid;
            }
            else if (string.Compare(value, "Started", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosEventKind.Started;
            }
            else if (string.Compare(value, "ExecutingFaults", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosEventKind.ExecutingFaults;
            }
            else if (string.Compare(value, "Waiting", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosEventKind.Waiting;
            }
            else if (string.Compare(value, "ValidationFailed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosEventKind.ValidationFailed;
            }
            else if (string.Compare(value, "TestError", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosEventKind.TestError;
            }
            else if (string.Compare(value, "Stopped", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ChaosEventKind.Stopped;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ChaosEventKind? value)
        {
            switch (value)
            {
                case ChaosEventKind.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ChaosEventKind.Started:
                    writer.WriteStringValue("Started");
                    break;
                case ChaosEventKind.ExecutingFaults:
                    writer.WriteStringValue("ExecutingFaults");
                    break;
                case ChaosEventKind.Waiting:
                    writer.WriteStringValue("Waiting");
                    break;
                case ChaosEventKind.ValidationFailed:
                    writer.WriteStringValue("ValidationFailed");
                    break;
                case ChaosEventKind.TestError:
                    writer.WriteStringValue("TestError");
                    break;
                case ChaosEventKind.Stopped:
                    writer.WriteStringValue("Stopped");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ChaosEventKind");
            }
        }
    }
}
