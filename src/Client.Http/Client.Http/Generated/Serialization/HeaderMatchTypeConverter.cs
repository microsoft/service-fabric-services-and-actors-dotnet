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
    /// Converter for <see cref="HeaderMatchType" />.
    /// </summary>
    internal class HeaderMatchTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static HeaderMatchType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(HeaderMatchType);

            if (string.Compare(value, "exact", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = HeaderMatchType.Exact;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, HeaderMatchType? value)
        {
            switch (value)
            {
                case HeaderMatchType.Exact:
                    writer.WriteStringValue("exact");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type HeaderMatchType");
            }
        }
    }
}
