// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric.Common;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class BinaryReaderWriterExtensions
    {
        public const byte NullPrefixByte = 0;
        public const byte NotNullPrefixByte = 1;
        private const int NegativeLength = -1;

        public static void Write(this BinaryWriter writer, Guid guid)
        {
            var gb = new GuidBytes(guid);

            writer.Write(gb.First64Bits);
            writer.Write(gb.Second64Bits);
        }

        public static Guid ReadGuid(this BinaryReader reader)
        {
            return new GuidBytes(reader.ReadUInt64(), reader.ReadUInt64()).Guid;
        }

        public static void Write(this BinaryWriter writer, ActorId actorId)
        {
            if (actorId == null)
            {
                writer.WriteNullPrefixByte();
                return;
            }

            writer.WriteNotNullPrefixByte();
            writer.Write((byte)actorId.Kind);

            switch (actorId.Kind)
            {
                case ActorIdKind.Long:
                    writer.Write(actorId.GetLongId());
                    break;
                case ActorIdKind.Guid:
                    writer.Write(actorId.GetGuidId());
                    break;
                case ActorIdKind.String:
                    writer.Write(actorId.GetStringId());
                    break;
                default:
                    ReleaseAssert.Failfast("The ActorIdKind value {0} is invalid", actorId.Kind);
                    break;
            }
        }

        public static ActorId ReadActorId(this BinaryReader reader)
        {
            if (reader.ReadByte() == NullPrefixByte)
            {
                return null;
            }

            var actorIdKind = (ActorIdKind)reader.ReadByte();

            switch (actorIdKind)
            {
                case ActorIdKind.Long:
                    return new ActorId(reader.ReadInt64());
                case ActorIdKind.Guid:
                    return new ActorId(reader.ReadGuid());
                case ActorIdKind.String:
                    return new ActorId(reader.ReadString());
                default:
                    ReleaseAssert.Failfast("The ActorIdKind value {0} is invalid", actorIdKind);
                    return null; // unreachable.
            }
        }

        public static void Write(this BinaryWriter writer, TimeSpan timeSpan)
        {
            writer.Write(timeSpan.Ticks);
        }

        public static TimeSpan ReadTimeSpan(this BinaryReader reader)
        {
            return TimeSpan.FromTicks(reader.ReadInt64());
        }

        public static void Write(this BinaryWriter writer, DateTime dateTime)
        {
            writer.Write(dateTime.Ticks);
        }

        public static DateTime ReadDateTime(this BinaryReader reader)
        {
            return new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
        }

        public static void Write(this BinaryWriter writer, string str, Encoding encoding)
        {
            if (str == null)
            {
                writer.Write(NegativeLength);
                return;
            }

            var bytes = encoding.GetBytes(str);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public static string ReadString(this BinaryReader reader, Encoding encoding)
        {
            var length = reader.ReadInt32();
            return length == NegativeLength ? null : encoding.GetString(reader.ReadBytes(length));
        }

        public static void WriteByteArray(this BinaryWriter writer, byte[] byteArr)
        {
            if (byteArr == null)
            {
                writer.Write(NegativeLength);
                return;
            }

            writer.Write(byteArr.Length);
            writer.Write(byteArr);
        }

        public static byte[] ReadByteArray(this BinaryReader reader)
        {
            var length = reader.ReadInt32();
            return length == NegativeLength ? null : reader.ReadBytes(length);
        }

        public static void WriteNullPrefixByte(this BinaryWriter writer)
        {
            writer.Write(NullPrefixByte);
        }

        public static void WriteNotNullPrefixByte(this BinaryWriter writer)
        {
            writer.Write(NotNullPrefixByte);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
#pragma warning disable SA1201 // Elements should appear in the correct order
    internal struct GuidBytes
#pragma warning restore SA1201 // Elements should appear in the correct order
    {
        [FieldOffset(0)]
        public readonly Guid Guid;

        [FieldOffset(0)]
        public readonly ulong First64Bits;

        [FieldOffset(8)]
        public readonly ulong Second64Bits;

        public GuidBytes(Guid guid)
            : this()
        {
            this.Guid = guid;
        }

        public GuidBytes(ulong first64Bits, ulong second64Bits)
            : this()
        {
            this.First64Bits = first64Bits;
            this.Second64Bits = second64Bits;
        }
    }
}
