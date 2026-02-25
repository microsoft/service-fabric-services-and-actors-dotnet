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
    /// Converter for <see cref="Validation" />.
    /// </summary>
    internal class ValidationConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static Validation? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(Validation);

            if (string.Compare(value, "Disabled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = Validation.Disabled;
            }
            else if (string.Compare(value, "Enabled", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = Validation.Enabled;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, Validation? value)
        {
            switch (value)
            {
                case Validation.Disabled:
                    writer.WriteStringValue("Disabled");
                    break;
                case Validation.Enabled:
                    writer.WriteStringValue("Enabled");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type Validation");
            }
        }
    }
}
