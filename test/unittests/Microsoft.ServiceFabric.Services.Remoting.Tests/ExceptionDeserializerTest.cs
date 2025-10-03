// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Fuzzy;
using Inspector;
using Xunit;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    public abstract class ExceptionDeserializerTest
    {
        // Test fixture
        private static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
        private readonly FabricTransportRemotingSettings remotingSettings = new FabricTransportRemotingSettings();

        public class Constructor : ExceptionDeserializerTest
        {
            [Fact]
            public void UsesProvidedExceptionConvertors()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new SystemExceptionConvertor(), };
                var deserializer = new ExceptionConversionHandler(customConvertors, this.remotingSettings);

                // Assert
                Assert.Same(customConvertors, deserializer.Field<IEnumerable<IExceptionConvertor>>().Value);
            }
        }

        public class CreateDefault : ExceptionDeserializerTest
        {
            [Fact]
            public void AppendsDefaultConvertorsToCustomList()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new FabricExceptionConvertor() };

                // Act
                ExceptionConversionHandler deserializer = ExceptionConversionHandler.CreateDefault(customConvertors, this.remotingSettings);

                // Assert
                IEnumerable<IExceptionConvertor> actualConvertors = deserializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.Equal(3, actualConvertors.Count());
                Assert.IsType<FabricExceptionConvertor>(actualConvertors.ElementAt(0)); // custom
                Assert.IsType<SystemExceptionConvertor>(actualConvertors.ElementAt(1)); // default
                Assert.IsType<FabricExceptionConvertor>(actualConvertors.ElementAt(2)); // default
            }

            [Fact]
            public void UsesDefaultConvertorsIfNullPassed()
            {
                // Arrange
                var expectedConvertors = new List<IExceptionConvertor>
                {
                    new SystemExceptionConvertor(),
                    new FabricExceptionConvertor(),
                };

                // Act
                ExceptionConversionHandler deserializer = ExceptionConversionHandler.CreateDefault(null, this.remotingSettings);

                // Assert
                IEnumerable<IExceptionConvertor> actualConvertors = deserializer.Field<IEnumerable<IExceptionConvertor>>().Value;

                Assert.Equal(expectedConvertors.Count, actualConvertors.Count());
                for (int i = 0; i < expectedConvertors.Count; i++)
                {
                    Assert.IsType(expectedConvertors[i].GetType(), actualConvertors.ElementAt(i));
                }
            }
        }

        public class DeserializeRemoteExceptionAndThrowAsync : ExceptionDeserializerTest
        {
            [Fact]
            public async Task ThrowsOriginalExceptionIfItIsKnownExceptionTypeAsync()
            {
                // Arrange
                var serializerRemotingSettings = new FabricTransport.Runtime.FabricTransportRemotingListenerSettings();
                var exceptionSerializer = Remoting.V2.Runtime.ExceptionConversionHandler.CreateDefault(Enumerable.Empty<Remoting.V2.Runtime.IExceptionConvertor>(), serializerRemotingSettings);
                var exceptionDeserializer = ExceptionConversionHandler.CreateDefault(Enumerable.Empty<IExceptionConvertor>(), this.remotingSettings);

                var originalException = new FabricInsufficientMaxLoadCapacityException(fuzzy.String());

                List<ArraySegment<byte>> serializedException = exceptionSerializer.SerializeRemoteException(originalException);
                var stream = new SegmentedReadMemoryStream(serializedException);

                // Act & Assert
                AggregateException exception = await Assert.ThrowsAsync<AggregateException>(async () =>
                    {
                        await exceptionDeserializer.DeserializeRemoteExceptionAndThrowAsync(stream);
                    });

                Exception innerException = exception.Flatten().InnerException;
                Assert.IsType<FabricInsufficientMaxLoadCapacityException>(innerException);
                Assert.Equal(originalException.Message, innerException.Message);
            }

            [Fact]
            public async Task ThrowsServiceExceptionForUnknownExceptions()
            {
                // Arrange
                var serializerRemotingSettings = new FabricTransport.Runtime.FabricTransportRemotingListenerSettings();
                var exceptionSerializer = Remoting.V2.Runtime.ExceptionConversionHandler.CreateDefault(Enumerable.Empty<Remoting.V2.Runtime.IExceptionConvertor>(), serializerRemotingSettings);
                var exceptionDeserializer = ExceptionConversionHandler.CreateDefault(Enumerable.Empty<IExceptionConvertor>(), this.remotingSettings);

                var originalException = new UnknownException(fuzzy.String());

                List<ArraySegment<byte>> serializedException = exceptionSerializer.SerializeRemoteException(originalException);
                var stream = new SegmentedReadMemoryStream(serializedException);

                // Act & Assert
                AggregateException exception = await Assert.ThrowsAsync<AggregateException>(async () =>
                    {
                        await exceptionDeserializer.DeserializeRemoteExceptionAndThrowAsync(stream);
                    });

                Exception innerException = exception.Flatten().InnerException;
                Assert.IsType<ServiceException>(innerException);
                Assert.Equal(originalException.Message, innerException.Message);
            }
        }

        private class UnknownException : Exception
        {
            public UnknownException(string message) : base(message) { }
        }
    }
}