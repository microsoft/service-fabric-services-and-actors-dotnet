// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class ByteArrayConverter
    {
        internal static byte[] Deserialize(JsonReader reader) =>
            reader.ReadList(JsonReaderExtensions.ReadValueAsByte).ToArray();

        internal static void Serialize(JsonWriter writer, byte[] bytes)
        {
            // write byte array as array of integers.
            if (bytes == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();
                foreach (byte item in bytes)
                    writer.WriteByteValue(item);
                writer.WriteEndArray();
            }
        }
    }
}
