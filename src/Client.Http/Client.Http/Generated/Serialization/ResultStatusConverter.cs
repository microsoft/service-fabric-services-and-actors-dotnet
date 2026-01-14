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
    /// Converter for <see cref="ResultStatus" />.
    /// </summary>
    internal class ResultStatusConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ResultStatus? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ResultStatus);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResultStatus.Invalid;
            }
            else if (string.Compare(value, "Succeeded", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResultStatus.Succeeded;
            }
            else if (string.Compare(value, "Cancelled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResultStatus.Cancelled;
            }
            else if (string.Compare(value, "Interrupted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResultStatus.Interrupted;
            }
            else if (string.Compare(value, "Failed", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResultStatus.Failed;
            }
            else if (string.Compare(value, "Pending", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ResultStatus.Pending;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ResultStatus? value)
        {
            switch (value)
            {
                case ResultStatus.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case ResultStatus.Succeeded:
                    writer.WriteStringValue("Succeeded");
                    break;
                case ResultStatus.Cancelled:
                    writer.WriteStringValue("Cancelled");
                    break;
                case ResultStatus.Interrupted:
                    writer.WriteStringValue("Interrupted");
                    break;
                case ResultStatus.Failed:
                    writer.WriteStringValue("Failed");
                    break;
                case ResultStatus.Pending:
                    writer.WriteStringValue("Pending");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ResultStatus");
            }
        }
    }
}
