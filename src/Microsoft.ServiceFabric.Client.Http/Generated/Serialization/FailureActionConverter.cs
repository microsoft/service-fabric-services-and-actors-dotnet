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
    /// Converter for <see cref="FailureAction" />.
    /// </summary>
    internal class FailureActionConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static FailureAction? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(FailureAction);

            if (string.Compare(value, "Invalid", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FailureAction.Invalid;
            }
            else if (string.Compare(value, "Rollback", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FailureAction.Rollback;
            }
            else if (string.Compare(value, "Manual", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FailureAction.Manual;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, FailureAction? value)
        {
            switch (value)
            {
                case FailureAction.Invalid:
                    writer.WriteStringValue("Invalid");
                    break;
                case FailureAction.Rollback:
                    writer.WriteStringValue("Rollback");
                    break;
                case FailureAction.Manual:
                    writer.WriteStringValue("Manual");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type FailureAction");
            }
        }
    }
}
