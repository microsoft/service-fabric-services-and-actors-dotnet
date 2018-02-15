// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Fabric.Common;
    using System.IO;
    using System.Text;

    internal sealed class LogicalTimestampSerializer
    {
        private const ushort DataVersionOne = 1;
        private const ushort CurrentDataVersion = DataVersionOne;
        private static readonly Encoding DataEncoding = Encoding.UTF8;

        internal static byte[] Serialize(LogicalTimestamp logicalTimestamp)
        {
            var dataSizeInBytes = ComputeSizeInBytes(logicalTimestamp);

            using (var stream = new MemoryStream(dataSizeInBytes))
            {
                using (var writer = new BinaryWriter(stream, DataEncoding))
                {
                    writer.Write(CurrentDataVersion);

                    if (logicalTimestamp == null)
                    {
                        writer.WriteNullPrefixByte();
                    }
                    else
                    {
                        writer.WriteNotNullPrefixByte();
                        writer.Write(logicalTimestamp.Timestamp);
                    }

                    writer.Flush();
                    return stream.GetBuffer();
                }
            }
        }

        internal static LogicalTimestamp Deserialize(byte[] logicalTimestampBytes)
        {
            if ((logicalTimestampBytes == null) || (logicalTimestampBytes.Length == 0))
            {
                return null;
            }

            using (var stream = new MemoryStream(logicalTimestampBytes))
            {
                using (var reader = new BinaryReader(stream, DataEncoding))
                {
                    var dataVersion = reader.ReadUInt16();
                    ReleaseAssert.AssertIfNot(dataVersion >= DataVersionOne, "Invalid data version: {0}", dataVersion);

                    if (reader.ReadByte() == BinaryReaderWriterExtensions.NullPrefixByte)
                    {
                        return null;
                    }

                    return new LogicalTimestamp(reader.ReadTimeSpan());
                }
            }
        }

        private static int ComputeSizeInBytes(LogicalTimestamp logicalTimestamp)
        {
            var size = sizeof(ushort); // Data version
            size += sizeof(byte); // Null value indicator prefix

            if (logicalTimestamp != null)
            {
                size += sizeof(long); // TimSpan
            }

            return size;
        }
    }
}
