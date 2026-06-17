// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Generator
{
    using Microsoft.ServiceFabric.Actors.Generator;
    using Xunit;

    /// <summary>
    /// Class containing tests for ACtorNameFormat.
    /// </summary>
    public class ActorNameFormatTests
    {
        /// <summary>
        /// Tests ActorNameFormat.GetFabricServiceName without providing a service name.
        /// </summary>
        [Fact]
        public void GetFabricService_NoServiceNameProvided_ReturnServiceName()
        {
            // Arrange
            var serviceName = "ObjectActorService";

            // Act
            var result = ActorNameFormat.GetFabricServiceName(typeof(object));

            // Assert
            Assert.Equal(serviceName, result);
        }

        /// <summary>
        /// Tests ActorNameFormat.GetFabricServiceName with providing a service name.
        /// </summary>
        [Fact]
        public void GetFabricService_PassServiceName_ReturnServiceName()
        {
            // Arrange
            var serviceName = "serviceName";

            // Act
            var result = ActorNameFormat.GetFabricServiceName(typeof(object), serviceName);

            // Assert
            Assert.Equal(serviceName, result);
        }
    }
}
