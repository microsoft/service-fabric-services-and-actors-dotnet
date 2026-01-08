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
    /// Converter for <see cref="ReplicatorOperationName" />.
    /// </summary>
    internal class ReplicatorOperationNameConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ReplicatorOperationName? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ReplicatorOperationName);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.Invalid;
            }
            else if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.None;
            }
            else if (string.Compare(value, "Open", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.Open;
            }
            else if (string.Compare(value, "ChangeRole", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.ChangeRole;
            }
            else if (string.Compare(value, "UpdateEpoch", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.UpdateEpoch;
            }
            else if (string.Compare(value, "Close", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.Close;
            }
            else if (string.Compare(value, "Abort", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.Abort;
            }
            else if (string.Compare(value, "OnDataLoss", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.OnDataLoss;
            }
            else if (string.Compare(value, "WaitForCatchup", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.WaitForCatchup;
            }
            else if (string.Compare(value, "Build", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ReplicatorOperationName.Build;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ReplicatorOperationName? value)
        {
            switch (value)
            {
                case ReplicatorOperationName.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ReplicatorOperationName.None:
                    writer.WriteStringValue("None");
                    break;
                case ReplicatorOperationName.Open:
                    writer.WriteStringValue("Open");
                    break;
                case ReplicatorOperationName.ChangeRole:
                    writer.WriteStringValue("ChangeRole");
                    break;
                case ReplicatorOperationName.UpdateEpoch:
                    writer.WriteStringValue("UpdateEpoch");
                    break;
                case ReplicatorOperationName.Close:
                    writer.WriteStringValue("Close");
                    break;
                case ReplicatorOperationName.Abort:
                    writer.WriteStringValue("Abort");
                    break;
                case ReplicatorOperationName.OnDataLoss:
                    writer.WriteStringValue("OnDataLoss");
                    break;
                case ReplicatorOperationName.WaitForCatchup:
                    writer.WriteStringValue("WaitForCatchup");
                    break;
                case ReplicatorOperationName.Build:
                    writer.WriteStringValue("Build");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ReplicatorOperationName");
            }
        }
    }
}
