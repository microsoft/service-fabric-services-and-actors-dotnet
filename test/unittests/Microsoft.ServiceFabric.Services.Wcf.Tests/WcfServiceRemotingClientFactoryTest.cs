// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Moq;
using Inspector;
using Xunit;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client
{
    public abstract class WcfServiceRemotingClientFactoryTest
    {
        public class Constructor
        {
            public class WhenExceptionConvertorsArePassed
            {
                [Fact]
                public void UsesPassedExceptionConvertors()
                {
                    // Arrange
                    var exceptionConvertors = new List<IExceptionConvertor>
                    {
                        new SystemExceptionConvertor(),
                    };

                    ExceptionDeserializer expectedExceptionDeserializer = ExceptionDeserializer.CreateSystemAndFabricExceptionDeserializer(exceptionConvertors);

                    var mockBinding = Mock.Of<System.ServiceModel.Channels.Binding>();
                    var factory = new WcfServiceRemotingClientFactory(
                        mockBinding,
                        null, // callbackClient
                        null, // exceptionHandlers
                        exceptionConvertors
                    );

                    // Act: Extract the deserializer from the factory (using reflection if needed), then extract convertors from the deserializer via property or method
                    ExceptionDeserializer actualExceptionDeserializer = factory.Field<ExceptionDeserializer>().Value;

                    // Assert: The deserializer in the factory should contain the same convertors as passed to the factory
                    Assert.NotNull(actualExceptionDeserializer);

                    // Extract convertors from both deserializers using reflection
                    IEnumerable<IExceptionConvertor> actualConvertors = actualExceptionDeserializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                    IEnumerable<IExceptionConvertor> expectedConvertors = expectedExceptionDeserializer.Field<IEnumerable<IExceptionConvertor>>().Value;

                    Assert.NotNull(actualConvertors);
                    Assert.NotNull(expectedConvertors);

                    // Compare that the types of the convertors in both lists are the same
                    var actualTypes = new List<Type>();
                    foreach (var c in actualConvertors) actualTypes.Add(c.GetType());
                    var expectedTypes = new List<Type>();
                    foreach (var c in expectedConvertors) expectedTypes.Add(c.GetType());
                    Assert.Equal(expectedTypes, actualTypes);
                }
            }

            public class WhenExceptionConvertorsAreNotPassed
            {
                [Fact]
                public void UsesDefaultExceptionConvertors()
                {
                    // Arrange
                    var mockBinding = new Mock<System.ServiceModel.Channels.Binding>();
                    var factory = new WcfServiceRemotingClientFactory(
                        mockBinding.Object,
                        null, // callbackClient
                        null, // exceptionHandlers
                        null // exceptionConvertors not passed
                    );

                    // Act: Extract the deserializer from the factory
                    var deserializerField = typeof(WcfServiceRemotingClientFactory).GetField("exceptionDeserializer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var actualExceptionDeserializer = deserializerField?.GetValue(factory) as ExceptionDeserializer;

                    // Assert: The deserializer in the factory should not be null
                    Assert.NotNull(actualExceptionDeserializer);

                    // Extract convertors from deserializer using reflection
                    var convertorsField = actualExceptionDeserializer.GetType().GetField("convertors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var actualConvertors = convertorsField?.GetValue(actualExceptionDeserializer) as IEnumerable<IExceptionConvertor>;

                    Assert.NotNull(actualConvertors);
                    var convertorList = new List<IExceptionConvertor>(actualConvertors);
                    Assert.Equal(2, convertorList.Count);
                    // Check that default convertors are present and in the expected order
                    Assert.IsType<SystemExceptionConvertor>(convertorList[0]);
                    Assert.IsType<FabricExceptionConvertor>(convertorList[1]);
                }
            }
        }
    }
}