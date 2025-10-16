// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Fuzzy;
using Inspector;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    public abstract class ExceptionSerializerTest
    {
        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public class Constructor : ExceptionSerializerTest
        {
            [Fact]
            public void UsesProvidedExceptionConvertors()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new SystemExceptionConvertor() };
                var serializer = new ExceptionConversionHandler(customConvertors, new FabricTransportRemotingListenerSettings());

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
                ExceptionConversionHandler serializer = ExceptionConversionHandler.CreateDefault(customConvertors, new FabricTransportRemotingListenerSettings());

                // Assert
                IEnumerable<IExceptionConvertor> actualConvertors = serializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.Equal(4, actualConvertors.Count());
                Assert.IsType<FabricExceptionConvertor>(actualConvertors.ElementAt(0)); // custom
                Assert.IsType<SystemExceptionConvertor>(actualConvertors.ElementAt(1)); // default
                Assert.IsType<FabricExceptionConvertor>(actualConvertors.ElementAt(2)); // default
                Assert.IsType<DefaultExceptionConvertor>(actualConvertors.ElementAt(3)); // default
            }

            [Fact]
            public void UsesDefaultConvertorsIfNullPassed()
            {
                // Arrange
                var expectedConvertors = new List<IExceptionConvertor>
                {
                    new SystemExceptionConvertor(),
                    new FabricExceptionConvertor(),
                    new DefaultExceptionConvertor()
                };

                // Act
                ExceptionConversionHandler serializer = ExceptionConversionHandler.CreateDefault(null, new FabricTransportRemotingListenerSettings());

                // Assert
                IEnumerable<IExceptionConvertor> actualConvertors = serializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                var expectedTypes = actualConvertors.Select(c => c.GetType()).ToArray();
                var actualTypes = expectedConvertors.Select(c => c.GetType()).ToArray();

                Assert.Equal(expectedTypes, actualTypes);
            }

            [Fact]
            public void UsesPassedRemotingListenerSettings()
            {
                // Arrange
                var expectedSettings = new FabricTransportRemotingListenerSettings();

                // Act
                ExceptionConversionHandler serializer = ExceptionConversionHandler.CreateDefault(null, expectedSettings);

                // Assert
                Assert.Same(expectedSettings, serializer.Field<IExceptionSerializerSettings>().Value);
            }
        }
    }
}
