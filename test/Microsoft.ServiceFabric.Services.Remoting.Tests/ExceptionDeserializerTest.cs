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
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Moq;
using Fuzzy;
using Inspector;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    public abstract class ExceptionDeserializerTest
    {
        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        Remoting.V2.Runtime.ExceptionSerializer exceptionSerializer = Remoting.V2.Runtime.ExceptionSerializer
                .CreateDefault(Enumerable.Empty<Remoting.V2.Runtime.IExceptionConvertor>(), Mock.Of<Remoting.V2.Runtime.IExceptionSerializerSettings>());

        public class Constructor : ExceptionDeserializerTest
        {
            [Fact]
            public void UsesProvidedExceptionConvertors()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new SystemExceptionConvertor(), };
                var deserializer = new ExceptionDeserializer(customConvertors);

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
                ExceptionDeserializer deserializer = ExceptionDeserializer.CreateDefault(customConvertors);

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
                ExceptionDeserializer deserializer = ExceptionDeserializer.CreateDefault(null);

                // Assert
                IEnumerable<IExceptionConvertor> actualConvertors = deserializer.Field<IEnumerable<IExceptionConvertor>>().Value;

                Assert.Equal(expectedConvertors.Count, actualConvertors.Count());
                for (int i = 0; i < expectedConvertors.Count; i++)
                {
                    Assert.IsType(expectedConvertors[i].GetType(), actualConvertors.ElementAt(i));
                }
            }
        }

        public class ConvertRemoteException : ExceptionDeserializerTest
        {
            [Fact]
            public void ReturnsOriginalExceptionIfItIsKnownExceptionType()
            {
                // Arrange
                var exceptionDeserializer = ExceptionDeserializer.CreateDefault(Enumerable.Empty<IExceptionConvertor>());

                var originalException = new FabricInsufficientMaxLoadCapacityException(fuzzy.String());

                RemoteException2 serializedException = exceptionSerializer.BuildRemoteException(originalException);

                // Act
                Exception resultException = exceptionDeserializer.ConvertRemoteException(serializedException);

                // Assert
                Assert.IsType<AggregateException>(resultException);
                Exception innerException = ((AggregateException)resultException).Flatten().InnerException;
                Assert.IsType<FabricInsufficientMaxLoadCapacityException>(innerException);
                Assert.Equal(originalException.Message, innerException.Message);
            }

            [Fact]
            public void ReturnsServiceExceptionForUnknownExceptions()
            {
                // Arrange
                var exceptionDeserializer = ExceptionDeserializer.CreateDefault(Enumerable.Empty<IExceptionConvertor>());

                var originalException = new UnknownException(fuzzy.String());

                RemoteException2 serializedException = exceptionSerializer.BuildRemoteException(originalException);

                // Act
                Exception resultException = exceptionDeserializer.ConvertRemoteException(serializedException);

                // Assert
                Assert.IsType<AggregateException>(resultException);
                Exception innerException = ((AggregateException)resultException).Flatten().InnerException;
                Assert.IsType<ServiceException>(innerException);
                Assert.Equal(originalException.Message, innerException.Message);
            }
        }

        public class DeserializeRemoteExceptionAndThrowAsync : ExceptionDeserializerTest
        {
            [Fact]
            public async Task ThrowsOriginalExceptionIfItIsKnownExceptionTypeAsync()
            {
                // Arrange
                var exceptionDeserializer = ExceptionDeserializer.CreateDefault(Enumerable.Empty<IExceptionConvertor>());

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
                var exceptionDeserializer = ExceptionDeserializer.CreateDefault(Enumerable.Empty<IExceptionConvertor>());

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
