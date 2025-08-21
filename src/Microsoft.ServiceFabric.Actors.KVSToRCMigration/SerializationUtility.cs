// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Xml;

    internal static class SerializationUtility
    {
        public static byte[] Serialize<T>(DataContractSerializer serializer, T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = XmlDictionaryWriter.CreateTextWriter(memoryStream))
                {
                    serializer.WriteObject(binaryWriter, obj);
                    binaryWriter.Flush();

                    return memoryStream.ToArray();
                }
            }
        }

        public static T Deserialize<T>(DataContractSerializer serializer, byte[] buffer)
        {
            using (Stream memoryStream = new MemoryStream())
            {
                memoryStream.Write(buffer, 0, buffer.Length);
                memoryStream.Position = 0;

                using (var reader = XmlDictionaryReader.CreateTextReader(memoryStream, XmlDictionaryReaderQuotas.Max))
                {
                    return (T)serializer.ReadObject(reader);
                }
            }
        }

        public static byte[] Serialize<T>(DataContractJsonSerializer serializer, T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);

                return memoryStream.ToArray();
            }
        }

        public static T Deserialize<T>(DataContractJsonSerializer serializer, byte[] buffer)
        {
            using (var memoryStream = new MemoryStream(buffer))
            {
                return (T)serializer.ReadObject(memoryStream);
            }
        }
    }
}
