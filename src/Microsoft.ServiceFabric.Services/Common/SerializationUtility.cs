// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Common
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    internal static class SerializationUtility
    {
        public static byte[] Serialize(DataContractSerializer serializer, object msg)
        {
            if (msg == null)
            {
                return null;
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    serializer.WriteObject(writer, msg);
                    writer.Flush();
                    return stream.ToArray();
                }
            }
        }

        public static object Deserialize(DataContractSerializer serializer, byte[] buffer)
        {
            if ((buffer == null) || (buffer.Length == 0))
            {
                return null;
            }

            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
                {
                    return serializer.ReadObject(reader);
                }
            }
        }
    }
}
