// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Converter for ByteArray
    /// </summary>
    internal class ByteArrayConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static byte[] Deserialize(JsonReader reader)
        {
            return reader.ReadList(JsonReaderExtensions.ReadValueAsByte).ToArray();
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="bytes">ByteArray to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, byte[] bytes)
        {
            // write byte array as array of integers.
            if (bytes == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartArray();
                foreach (var item in bytes)
                {
                    writer.WriteByteValue(item);
                }

                writer.WriteEndArray();
            }
        }
    }
}
