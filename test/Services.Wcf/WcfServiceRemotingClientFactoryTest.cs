// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Moq;
using Inspector;
using Xunit;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using System.Linq;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client
{
    public abstract class WcfServiceRemotingClientFactoryTest
    {
        public class Constructor
        {
            [Fact]
            public void UsesPassedExceptionConvertors()
            {
                // Arrange
                var exceptionConvertors = new List<IExceptionConvertor>
                {
                    new SystemExceptionConvertor(),
                };

                ExceptionDeserializer expectedExceptionDeserializer = ExceptionDeserializer.CreateDefault(exceptionConvertors);

                var mockBinding = Mock.Of<System.ServiceModel.Channels.Binding>();

                // Act
                var factory = new WcfServiceRemotingClientFactory(
                    mockBinding,
                    null, // callbackClient
                    null, // exceptionHandlers
                    exceptionConvertors
                );

                ExceptionDeserializer actualExceptionDeserializer = factory.Field<ExceptionDeserializer>().Value;

                // Assert
                // Extract convertors from both deserializers
                IEnumerable<IExceptionConvertor> actualConvertors = actualExceptionDeserializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                IEnumerable<IExceptionConvertor> expectedConvertors = expectedExceptionDeserializer.Field<IEnumerable<IExceptionConvertor>>().Value;

                Assert.NotNull(actualConvertors);

                // Compare that the types of the convertors in both lists are the same
                Assert.Equal(expectedConvertors.Count(), actualConvertors.Count());
                for (int i = 0; i < actualConvertors.Count(); i++)
                {
                    Assert.Equal(expectedConvertors.ElementAt(i).GetType(), actualConvertors.ElementAt(i).GetType());
                }
            }
        }
    }
}