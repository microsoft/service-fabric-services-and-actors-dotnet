// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Xunit;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    public abstract class ExceptionSerializerTest
    {
        public class Constructor
        {
            [Fact]
            public void UsesProvidedExceptionConvertors()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new SystemExceptionConvertor() };
                var serializer = new ExceptionSerializer(customConvertors, null);

                // Assert
                Assert.NotNull(serializer);
                IEnumerable<IExceptionConvertor> actualConvertors = serializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.NotNull(actualConvertors);
                var convertorList = new List<IExceptionConvertor>(actualConvertors);
                Assert.Single(convertorList);
                Assert.IsType<SystemExceptionConvertor>(convertorList[0]);
            }
        }

        public class CreateSystemAndFabricExceptionSerializer
        {
            [Fact]
            public void AddsDefaultConvertors()
            {
                // Act
                var serializer = ExceptionSerializer.CreateSystemAndFabricExceptionSerializer();

                // Assert
                Assert.NotNull(serializer);
                IEnumerable<IExceptionConvertor> actualConvertors = serializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.NotNull(actualConvertors);
                var convertorList = new List<IExceptionConvertor>(actualConvertors);
                Assert.Equal(3, convertorList.Count);
                Assert.IsType<SystemExceptionConvertor>(convertorList[0]);
                Assert.IsType<FabricExceptionConvertor>(convertorList[1]);
                Assert.IsType<DefaultExceptionConvertor>(convertorList[2]);
            }

            [Fact]
            public void AppendsDefaultConvertorsToCustomList()
            {
                // Arrange
                var customConvertors = new List<IExceptionConvertor> { new FabricExceptionConvertor() };

                // Act
                var serializer = ExceptionSerializer.CreateSystemAndFabricExceptionSerializer(customConvertors);

                // Assert
                Assert.NotNull(serializer);
                IEnumerable<IExceptionConvertor> actualConvertors = serializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                Assert.NotNull(actualConvertors);
                var convertorList = new List<IExceptionConvertor>(actualConvertors);
                Assert.Equal(4, convertorList.Count);
                Assert.IsType<FabricExceptionConvertor>(convertorList[0]); // custom
                Assert.IsType<SystemExceptionConvertor>(convertorList[1]); // default
                Assert.IsType<FabricExceptionConvertor>(convertorList[2]); // default
                Assert.IsType<DefaultExceptionConvertor>(convertorList[3]); // default
            }
        }
    }
}
