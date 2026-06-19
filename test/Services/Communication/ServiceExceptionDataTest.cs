// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication;

public abstract class ServiceExceptionDataTest
{
    readonly ServiceExceptionData sut;

    // Constructor parameters
    readonly string type = fuzzy.String();
    readonly string message = fuzzy.String();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServiceExceptionDataTest() =>
        sut = new ServiceExceptionData(type, message);

    public sealed class Constructor : ServiceExceptionDataTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(type, sut.Type);
            Assert.Same(message, sut.Message);
        }
    }

    public sealed class Serialization : ServiceExceptionDataTest
    {
        [Fact]
        public void RoundTripsTypeAndMessageThroughDataContractSerializer()
        {
            var serializer = new DataContractSerializer(typeof(ServiceExceptionData));
            using var stream = new MemoryStream();

            serializer.WriteObject(stream, sut);
            stream.Position = 0;
            var actual = (ServiceExceptionData)serializer.ReadObject(stream);

            Assert.Equal(type, actual.Type);
            Assert.Equal(message, actual.Message);
        }

        [Fact]
        public void UsesDataContractNameAndNamespace()
        {
            var serializer = new DataContractSerializer(typeof(ServiceExceptionData));
            using var stream = new MemoryStream();

            serializer.WriteObject(stream, sut);
            stream.Position = 0;
            using var reader = XmlReader.Create(stream);
            reader.MoveToContent();

            Assert.Equal("ServiceExceptionData", reader.LocalName);
            Assert.Equal("urn:ServiceFabric.Communication", reader.NamespaceURI);
        }
    }
}
