// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Xunit;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    public abstract class ExceptionDeserializerTest
    {
        public class Constructor
        {
            [Fact]
            public void UsesProvidedExceptionConvertors()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new SystemExceptionConvertor() };
                var deserializer = new ExceptionDeserializer(customConvertors);

                // Assert
                Assert.NotNull(deserializer);
                IEnumerable<IExceptionConvertor> actualConvertors = deserializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.NotNull(actualConvertors);
                var convertorList = new List<IExceptionConvertor>(actualConvertors);
                Assert.Single(convertorList);
                Assert.IsType<SystemExceptionConvertor>(convertorList[0]);
            }
        }

        public class CreateSystemAndFabricExceptionDeserializer
        {
            [Fact]
            public void AddsDefaultConvertors()
            {
                // Act
                var deserializer = ExceptionDeserializer.CreateSystemAndFabricExceptionDeserializer();

                // Assert
                Assert.NotNull(deserializer);
                IEnumerable<IExceptionConvertor> actualConvertors = deserializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.NotNull(actualConvertors);
                var convertorList = new List<IExceptionConvertor>(actualConvertors);
                Assert.Equal(2, convertorList.Count);
                Assert.IsType<SystemExceptionConvertor>(convertorList[0]);
                Assert.IsType<FabricExceptionConvertor>(convertorList[1]);
            }

            [Fact]
            public void AppendsDefaultConvertorsToCustomList()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new FabricExceptionConvertor() };

                // Act
                var deserializer = ExceptionDeserializer.CreateSystemAndFabricExceptionDeserializer(customConvertors);

                // Assert
                Assert.NotNull(deserializer);
                IEnumerable<IExceptionConvertor> actualConvertors = deserializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.NotNull(actualConvertors);
                var convertorList = new List<IExceptionConvertor>(actualConvertors);
                Assert.Equal(3, convertorList.Count);
                Assert.IsType<FabricExceptionConvertor>(convertorList[0]); // custom
                Assert.IsType<SystemExceptionConvertor>(convertorList[1]); // default
                Assert.IsType<FabricExceptionConvertor>(convertorList[2]); // default
            }
        }
    }
}
