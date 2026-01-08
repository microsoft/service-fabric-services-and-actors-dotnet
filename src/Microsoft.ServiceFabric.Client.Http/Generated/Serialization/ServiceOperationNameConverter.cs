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
    /// Converter for <see cref="ServiceOperationName" />.
    /// </summary>
    internal class ServiceOperationNameConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static ServiceOperationName? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(ServiceOperationName);

            if (string.Compare(value, "Unknown", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceOperationName.Unknown;
            }
            else if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceOperationName.None;
            }
            else if (string.Compare(value, "Open", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceOperationName.Open;
            }
            else if (string.Compare(value, "ChangeRole", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceOperationName.ChangeRole;
            }
            else if (string.Compare(value, "Close", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceOperationName.Close;
            }
            else if (string.Compare(value, "Abort", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = ServiceOperationName.Abort;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, ServiceOperationName? value)
        {
            switch (value)
            {
                case ServiceOperationName.Unknown:
                    writer.WriteStringValue("Unknown");
                    break;
                case ServiceOperationName.None:
                    writer.WriteStringValue("None");
                    break;
                case ServiceOperationName.Open:
                    writer.WriteStringValue("Open");
                    break;
                case ServiceOperationName.ChangeRole:
                    writer.WriteStringValue("ChangeRole");
                    break;
                case ServiceOperationName.Close:
                    writer.WriteStringValue("Close");
                    break;
                case ServiceOperationName.Abort:
                    writer.WriteStringValue("Abort");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type ServiceOperationName");
            }
        }
    }
}
