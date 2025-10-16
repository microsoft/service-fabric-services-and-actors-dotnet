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
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

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
                var deserializationSettings = new FabricTransportRemotingSettings();
                var exceptionConvertors = new List<IExceptionConvertor>
                {
                    new SystemExceptionConvertor(),
                };

                ExceptionConversionHandler expectedExceptionDeserializer = ExceptionConversionHandler.CreateDefault(exceptionConvertors, deserializationSettings);

                var mockBinding = Mock.Of<System.ServiceModel.Channels.Binding>();

                // Act
                var factory = new WcfServiceRemotingClientFactory(
                    mockBinding,
                    null, // callbackClient
                    null, // exceptionHandlers
                    exceptionConvertors
                );

                ExceptionConversionHandler actualExceptionDeserializer = factory.Field<ExceptionConversionHandler>().Value;

                // Assert
                // Extract convertors from both deserializers
                IEnumerable<IExceptionConvertor> actualConvertors = actualExceptionDeserializer.Field<IEnumerable<IExceptionConvertor>>().Value;
                IEnumerable<IExceptionConvertor> expectedConvertors = expectedExceptionDeserializer.Field<IEnumerable<IExceptionConvertor>>().Value;

                Assert.NotNull(actualConvertors);

                var expectedTypes = actualExceptionDeserializer.Field<IEnumerable<IExceptionConvertor>>().Value.Select(c => c.GetType()).ToArray();
                var actualTypes = expectedExceptionDeserializer.Field<IEnumerable<IExceptionConvertor>>().Value.Select(c => c.GetType()).ToArray();

                Assert.Equal(expectedTypes, actualTypes);
            }
        }
    }
}
