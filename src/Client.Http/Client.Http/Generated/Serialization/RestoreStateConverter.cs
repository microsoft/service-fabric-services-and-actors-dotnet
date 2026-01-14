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
    /// Converter for <see cref="RestoreState" />.
    /// </summary>
    internal class RestoreStateConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static RestoreState? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(RestoreState);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RestoreState.Invalid;
            }
            else if (string.Compare(value, "Accepted", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RestoreState.Accepted;
            }
            else if (string.Compare(value, "RestoreInProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RestoreState.RestoreInProgress;
            }
            else if (string.Compare(value, "SecondariesInProgress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RestoreState.SecondariesInProgress;
            }
            else if (string.Compare(value, "Success", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RestoreState.Success;
            }
            else if (string.Compare(value, "Failure", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RestoreState.Failure;
            }
            else if (string.Compare(value, "Timeout", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = RestoreState.Timeout;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, RestoreState? value)
        {
            switch (value)
            {
                case RestoreState.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case RestoreState.Accepted:
                    writer.WriteStringValue("Accepted");
                    break;
                case RestoreState.RestoreInProgress:
                    writer.WriteStringValue("RestoreInProgress");
                    break;
                case RestoreState.SecondariesInProgress:
                    writer.WriteStringValue("SecondariesInProgress");
                    break;
                case RestoreState.Success:
                    writer.WriteStringValue("Success");
                    break;
                case RestoreState.Failure:
                    writer.WriteStringValue("Failure");
                    break;
                case RestoreState.Timeout:
                    writer.WriteStringValue("Timeout");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type RestoreState");
            }
        }
    }
}
