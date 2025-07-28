// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Fuzzy;
using Inspector;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    public abstract class ExceptionSerializerTest
    {
        // Text fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public class Constructor : ExceptionSerializerTest
        {
            [Fact]
            public void UsesProvidedExceptionConvertors()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new SystemExceptionConvertor() };
                var serializer = new ExceptionSerializer(customConvertors, null);

                // Assert
                Assert.Same(customConvertors, serializer.Field<IEnumerable<IExceptionConvertor>>().Value);
            }
        }

        public class CreateDefault : ExceptionSerializerTest
        {
            [Fact]
            public void AppendsDefaultConvertorsToCustomList()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new FabricExceptionConvertor() };

                // Act
                ExceptionSerializer serializer = ExceptionSerializer.CreateDefault(customConvertors);

                // Assert
                IEnumerable<IExceptionConvertor> actualConvertors = serializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.Equal(4, actualConvertors.Count());
                Assert.IsType<FabricExceptionConvertor>(actualConvertors.ElementAt(0)); // custom
                Assert.IsType<SystemExceptionConvertor>(actualConvertors.ElementAt(1)); // default
                Assert.IsType<FabricExceptionConvertor>(actualConvertors.ElementAt(2)); // default
                Assert.IsType<DefaultExceptionConvertor>(actualConvertors.ElementAt(3)); // default
            }
        }

        public class SerializeRemoteException : ExceptionSerializerTest
        {
            [Fact]
            public void BuildsOriginalExceptionIfItIsKnownExceptionType()
            {
                // Arrange
                var exceptionSerializer = ExceptionSerializer.CreateDefault(Enumerable.Empty<IExceptionConvertor>());
                var exceptionDeserializer = Remoting.V2.Client.ExceptionDeserializer.CreateDefault(Enumerable.Empty<Remoting.V2.Client.IExceptionConvertor>());
                var originalException = new FabricInsufficientMaxLoadCapacityException(fuzzy.String());

                // Act
                RemoteException2 serializedException = exceptionSerializer.BuildRemoteException(originalException);

                // Assert
                Exception resultException = exceptionDeserializer.ConvertRemoteException(serializedException);
                Assert.IsType<AggregateException>(resultException);
                Exception innerException = ((AggregateException)resultException).Flatten().InnerException;
                Assert.IsType<FabricInsufficientMaxLoadCapacityException>(innerException);
                Assert.Equal(originalException.Message, innerException.Message);
            }

            [Fact]
            public void BuildsServiceExceptionIfItIsNotKnownExceptionType()
            {
                // Arrange
                var exceptionSerializer = ExceptionSerializer.CreateDefault(Enumerable.Empty<IExceptionConvertor>());
                var exceptionDeserializer = Remoting.V2.Client.ExceptionDeserializer.CreateDefault(Enumerable.Empty<Remoting.V2.Client.IExceptionConvertor>());
                var originalException = new UnknownException(fuzzy.String());

                // Act
                RemoteException2 serializedException = exceptionSerializer.BuildRemoteException(originalException);

                // Assert
                Exception resultException = exceptionDeserializer.ConvertRemoteException(serializedException);
                Assert.IsType<AggregateException>(resultException);
                Exception innerException = ((AggregateException)resultException).Flatten().InnerException;
                Assert.IsType<ServiceException>(innerException);
                Assert.Equal(originalException.Message, innerException.Message);
            }
        }
        
        private class UnknownException : Exception
        {
            public UnknownException(string message) : base(message)
            {
            }
        }
    }
}
