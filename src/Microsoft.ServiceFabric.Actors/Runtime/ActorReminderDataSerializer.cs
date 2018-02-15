// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Fabric.Common;
    using System.IO;
    using System.Text;

    internal sealed class ActorReminderDataSerializer
    {
        private const ushort DataVersionOne = 1;
        private const ushort CurrentDataVersion = DataVersionOne;
        private static readonly Encoding DataEncoding = Encoding.UTF8;

        internal static byte[] Serialize(ActorReminderData reminderData)
        {
            var dataSizeInBytes = ComputeSizeInBytes(reminderData);

            using (var stream = new MemoryStream(dataSizeInBytes))
            {
                using (var writer = new BinaryWriter(stream, DataEncoding))
                {
                    writer.Write(CurrentDataVersion);

                    if (reminderData == null)
                    {
                        writer.WriteNullPrefixByte();
                    }
                    else
                    {
                        writer.WriteNotNullPrefixByte();
                        writer.Write(reminderData.ActorId);
                        writer.Write(reminderData.Name, DataEncoding);
                        writer.Write(reminderData.DueTime);
                        writer.Write(reminderData.Period);
                        writer.WriteByteArray(reminderData.State);
                        writer.Write(reminderData.LogicalCreationTime);
                    }

                    writer.Flush();
                    return stream.GetBuffer();
                }
            }
        }

        internal static ActorReminderData Deserialize(byte[] reminderDataBytes)
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

                    return new ActorReminderData(
                        reader.ReadActorId(),
                        reader.ReadString(DataEncoding),
                        reader.ReadTimeSpan(),
                        reader.ReadTimeSpan(),
                        reader.ReadByteArray(),
                        reader.ReadTimeSpan()
                    );
                }
            }
        }

        private static int ComputeSizeInBytes(ActorReminderData reminderData)
        {
            var size = sizeof(ushort); // Data version
            size += sizeof(byte); // Null value indicator prefix

            if (reminderData == null)
            {
                return size;
            }

            size += ComputeActorIdSize(reminderData.ActorId); // ActorId.
            size += ComputeStringSize(reminderData.Name); // Reminder name.
            size += sizeof(long); // Reminder due time.
            size += sizeof(long); // Reminder period.
            size += ComputeByteArraySize(reminderData.State); // Reminder state byte array.
            size += sizeof(long); // Reminder logical creation time.

            return size;
        }

        private static int ComputeActorIdSize(ActorId actorId)
        {
            var size = sizeof(byte); // Null indicator prefix

            if (actorId == null)
            {
                return size;
            }

            size += sizeof(byte); // ActorIdKind

            switch (actorId.Kind)
            {
                case ActorIdKind.Long:
                    size += sizeof(long);
                    break;
                case ActorIdKind.Guid:
                    size += 16; // Guid is stored as 128 bit value.
                    break;
                case ActorIdKind.String:
                    size += DataEncoding.GetByteCount(actorId.GetStringId());
                    size += sizeof(int); // String Id byte array lenth.
                    break;
                default:
                    ReleaseAssert.Failfast("The ActorIdKind value {0} is invalid", actorId.Kind);
                    size = -1; // unreachable
                    break;
            }

            return size;
        }

        private static int ComputeStringSize(string str)
        {
            var size = sizeof(int); // Actual length or negative length for null indication

            if (str != null)
            {
                size += DataEncoding.GetByteCount(str);
            }

            return size;
        }

        private static int ComputeByteArraySize(byte[] byteArr)
        {
            var size = sizeof(int); // Actual length or negative length for null indication

            if (byteArr != null)
            {
                size += byteArr.Length;
            }

            return size;
        }
    }
}
