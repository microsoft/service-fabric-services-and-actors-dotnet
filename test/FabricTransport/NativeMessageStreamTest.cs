// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric.Interop;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Microsoft.ServiceFabric.FabricTransport;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Messaging.Test
{
    public class NativeMessageStreamTest
    {
        [Fact]
        public static unsafe void SimpleReadingFromStreamTest()
        {
            var bytearr = Encoding.Unicode.GetBytes("Here is some data.");
            var bytearrayList = new List<byte[]>() {bytearr};
            var pin = new PinCollection();
            var msgStream = new NativeMessageStream(CreateNativeBufferPtr(bytearrayList, pin));
            Assert.True(msgStream.Length == bytearr.Length);
            var streambytes = new byte[msgStream.Length];
            var bytesread = msgStream.Read(streambytes, 0, streambytes.Length);
            Assert.True(bytearr.SequenceEqual(streambytes));
            Assert.True(bytesread == streambytes.Length);
        }

        [Fact]
        public static unsafe void SimpleReadingFromStreamTestSerializer()
        {
            var serializer = new DataContractSerializer(typeof(TestMessageBody));
            var memStream = new MemoryStream();
            var input = new byte[1024];

            for (var i = 0; i < input.Length; i++)
            {
                input[i] = (byte)i;
            }
            var writer = XmlDictionaryWriter.CreateBinaryWriter(memStream);
            
                serializer.WriteObject(writer, new TestMessageBody(input));

            var bytearrayList = new List<byte[]>() { memStream.ToArray() };
            var pin = new PinCollection();
            Console.WriteLine("Buffer length Input" + bytearrayList[0].Length);
            var msgStream = new NativeMessageStream(CreateNativeBufferPtr(bytearrayList, pin));
            byte[] arr = new byte[bytearrayList[0].Length];
            for (int i=0;i<arr.Length;i++)
            {
                arr[i] = (byte)msgStream.ReadByte();
            }
            Assert.True(arr.SequenceEqual(memStream.ToArray()));
          
        }

        [Fact]
        public static unsafe void ReadFromFullyReadStreamTest()
        {
            var bytearr = Encoding.Unicode.GetBytes("Here is some data.");
            var bytearrayList = new List<byte[]>() {bytearr};
            var pin = new PinCollection();
            var msgStream = new NativeMessageStream(CreateNativeBufferPtr(bytearrayList, pin));
            Assert.True(msgStream.Length == bytearr.Length);
            var streambytes = new byte[msgStream.Length];
            //First Read
            var bytesread = msgStream.Read(streambytes, 0, streambytes.Length);
            Assert.True(bytesread == streambytes.Length);
            //Second read
            bytesread = msgStream.Read(streambytes, 0, streambytes.Length);
            Assert.True(bytesread == 0);
        }

        [Fact]
        public static unsafe void SimpleReadingFromStreamTestWithMultipleByteArray()
        {
            var bytearr1 = Encoding.Unicode.GetBytes("This is first byte array");
            var bytearr2 = Encoding.Unicode.GetBytes("This is second byte array");
            var pin = new PinCollection();
            byte[] headerBytes = {4, 5};

            var bytearrayList = new List<byte[]>() {bytearr1, bytearr2};

            var nativebuffer = CreateNativeBufferPtr(bytearrayList, pin);

            var msgStream = new NativeMessageStream(nativebuffer);

            Assert.True(msgStream.Length == (bytearr1.Length + bytearr2.Length));
            var streambytes = new byte[msgStream.Length];
            var bytesread = msgStream.Read(streambytes, 0, streambytes.Length);
            Assert.True(bytesread == streambytes.Length);
        }

        [Fact]
        public static unsafe void ReadingInChunksFromMultipleByteArrayTest()
        {
            var bytearr1 = Encoding.Unicode.GetBytes("This is first byte array");
            var bytearr2 = Encoding.Unicode.GetBytes("This is second byte array");
            var pin = new PinCollection();
            var bytearrayList = new List<byte[]>() {bytearr1, bytearr2};
            var nativebuffer = CreateNativeBufferPtr(bytearrayList, pin);

            var msgStream = new NativeMessageStream(nativebuffer);
            Assert.True(msgStream.Length == (bytearr1.Length + bytearr2.Length));
            var streambytes = new byte[msgStream.Length];
            var bytesread1 = msgStream.Read(streambytes, 0, 25);
            Assert.True(bytesread1 == 25);
            Assert.True(msgStream.Position == bytesread1);

            var bytesread2 = msgStream.Read(streambytes, bytesread1, (int) msgStream.Length - bytesread1);
            Assert.True(msgStream.Position == bytesread1 + bytesread2);
            Assert.True((bytesread1 + bytesread2) == msgStream.Length);
        }

        [Fact]
        public static unsafe void OutOfArgumentExceptionTest()
        {
            var bytearr1 = Encoding.Unicode.GetBytes("This is first byte array");
            var bytearr2 = Encoding.Unicode.GetBytes("This is second byte array");
            var pin = new PinCollection();
            var bytearrayList = new List<byte[]>() {bytearr1, bytearr2};
            var nativebuffer = CreateNativeBufferPtr(bytearrayList, pin);

            var msgStream = new NativeMessageStream(nativebuffer);
            Assert.True(msgStream.Length == (bytearr1.Length + bytearr2.Length));
            var streambytes = new byte[msgStream.Length];
            try
            {
                var bytesread1 = msgStream.Read(streambytes, 0, (int) streambytes.Length + 1);
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        [Fact]
        public static unsafe void ReadingMoreBytesThanInStreamTest()
        {
            var bytearr1 = Encoding.Unicode.GetBytes("This is first byte array");
            var bytearr2 = Encoding.Unicode.GetBytes("This is second byte array");
            var pin = new PinCollection();
            var bytearrayList = new List<byte[]>() {bytearr1, bytearr2};
            var nativebuffer = CreateNativeBufferPtr(bytearrayList, pin);
            var msgStream = new NativeMessageStream(nativebuffer);
            Assert.True(msgStream.Length == (bytearr1.Length + bytearr2.Length));
            var streambytes = new byte[1024];

            var bytesread1 = msgStream.Read(streambytes, 0, (int) msgStream.Length + 1);
            Assert.True(bytesread1 > 0);
            Assert.True(bytesread1 == msgStream.Length);
        }

        private static List<Tuple<uint, IntPtr>> CreateNativeBufferPtr(List<byte[]> bytearrayList, PinCollection pin)
        {
            var nativeBuffers = new List<Tuple<uint, IntPtr>>(bytearrayList.Count);
            for (var i = 0; i < bytearrayList.Count; i++)
            {
                var nativePtr = NativeTypes.ToNativeBytes(pin, bytearrayList[i]);
                nativeBuffers.Add(nativePtr);
            }

            return nativeBuffers;
        }
    }

    [DataContract]
    internal class TestMessageBody
    {
        [DataMember]
        internal readonly byte[] input;

        public TestMessageBody(byte[] input)
        {
            this.input = input;
        }
    }
}
