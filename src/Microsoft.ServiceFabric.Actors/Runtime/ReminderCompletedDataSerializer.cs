// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Fabric.Common;
    using System.IO;
    using System.Text;

    internal sealed class ReminderCompletedDataSerializer
    {
        private const ushort DataVersionOne = 1;
        private const ushort CurrentDataVersion = DataVersionOne;
        private static readonly Encoding DataEncoding = Encoding.UTF8;

        internal static byte[] Serialize(ReminderCompletedData reminderCompletedData)
        {
            var dataSizeInBytes = ComputeSizeInBytes(reminderCompletedData);

            using (var stream = new MemoryStream(dataSizeInBytes))
            {
                using (var writer = new BinaryWriter(stream, DataEncoding))
                {
                    writer.Write(CurrentDataVersion);

                    if (reminderCompletedData == null)
                    {
                        writer.WriteNullPrefixByte();
                    }
                    else
                    {
                        writer.WriteNotNullPrefixByte();
                        writer.Write(reminderCompletedData.LogicalTime);
                        writer.Write(reminderCompletedData.UtcTime);
                    }

                    writer.Flush();
                    return stream.GetBuffer();
                }
            }
        }

        internal static ReminderCompletedData Deserialize(byte[] reminderDataBytes)
        {
            if ((reminderDataBytes == null) || (reminderDataBytes.Length == 0))
            {
                return null;
            }

            using (var stream = new MemoryStream(reminderDataBytes))
            {
                using (var reader = new BinaryReader(stream, DataEncoding))
                {
                    var dataVersion = reader.ReadUInt16();
                    ReleaseAssert.AssertIfNot(dataVersion >= DataVersionOne, "Invalid data version: {0}", dataVersion);

                    if (reader.ReadByte() == BinaryReaderWriterExtensions.NullPrefixByte)
                    {
                        return null;
                    }

                    return new ReminderCompletedData(reader.ReadTimeSpan(), reader.ReadDateTime());
                }
            }
        }

        private static int ComputeSizeInBytes(ReminderCompletedData reminderCompletedData)
        {
            int size = sizeof(ushort); // Data version
            size += sizeof(byte); // Null value indicator prefix

            if (reminderCompletedData != null)
            {
                size += 2 * sizeof(long); // LogicalTime + UtcTime;
            }

            return size;
        }
    }
}
