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
    /// Converter for <see cref="OperationState" />.
    /// </summary>
    internal class OperationStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static OperationState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(OperationState);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationState.Invalid;
            }
            else if (string.Compare(value, "Running", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationState.Running;
            }
            else if (string.Compare(value, "RollingBack", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationState.RollingBack;
            }
            else if (string.Compare(value, "Completed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationState.Completed;
            }
            else if (string.Compare(value, "Faulted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationState.Faulted;
            }
            else if (string.Compare(value, "Cancelled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationState.Cancelled;
            }
            else if (string.Compare(value, "ForceCancelled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = OperationState.ForceCancelled;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, OperationState? value)
        {
            switch (value)
            {
                case OperationState.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case OperationState.Running:
                    writer.WriteStringValue("Running");
                    break;
                case OperationState.RollingBack:
                    writer.WriteStringValue("RollingBack");
                    break;
                case OperationState.Completed:
                    writer.WriteStringValue("Completed");
                    break;
                case OperationState.Faulted:
                    writer.WriteStringValue("Faulted");
                    break;
                case OperationState.Cancelled:
                    writer.WriteStringValue("Cancelled");
                    break;
                case OperationState.ForceCancelled:
                    writer.WriteStringValue("ForceCancelled");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type OperationState");
            }
        }
    }
}
