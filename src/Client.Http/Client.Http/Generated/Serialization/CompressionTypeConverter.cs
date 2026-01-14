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
    /// Converter for <see cref="CompressionType" />.
    /// </summary>
    internal class CompressionTypeConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static CompressionType? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(CompressionType);

            if (string.Compare(value, "CLUSTER_DEFINED", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CompressionType.CLUSTER_DEFINED;
            }
            else if (string.Compare(value, "ZIP", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CompressionType.ZIP;
            }
            else if (string.Compare(value, "ZSTANDARD", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = CompressionType.ZSTANDARD;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, CompressionType? value)
        {
            switch (value)
            {
                case CompressionType.CLUSTER_DEFINED:
                    writer.WriteStringValue("CLUSTER_DEFINED");
                    break;
                case CompressionType.ZIP:
                    writer.WriteStringValue("ZIP");
                    break;
                case CompressionType.ZSTANDARD:
                    writer.WriteStringValue("ZSTANDARD");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type CompressionType");
            }
        }
    }
}
