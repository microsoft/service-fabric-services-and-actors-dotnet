// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Tests.Generator
{
    using Actors.Generator;
    using Xunit;

    public class ActorNameFormatTests
    {
        [Fact]
        public void GetFabricService_NoServiceNameProvided_ReturnServiceName()
        {
            // Arrange
            string serviceName = "ObjectActorService";

            // Act
            var result = ActorNameFormat.GetFabricServiceName(typeof(object));

            // Assert
            Assert.Equal(serviceName, result);
        }

        [Fact]
        public void GetFabricService_PassServiceName_ReturnServiceName()
        {
            // Arrange
            string serviceName = "serviceName";

            // Act
            var result = ActorNameFormat.GetFabricServiceName(typeof(object), serviceName);

            // Assert
            Assert.Equal(serviceName, result);
        }
    }
}