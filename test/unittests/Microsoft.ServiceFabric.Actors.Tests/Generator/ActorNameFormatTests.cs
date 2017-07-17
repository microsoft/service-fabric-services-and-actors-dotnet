// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Generator
{
    using Microsoft.ServiceFabric.Actors.Generator;
    using FluentAssertions;
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
            result.Should().Be(serviceName);
        }

        [Fact]
        public void GetFabricService_PassServiceName_ReturnServiceName()
        {
            // Arrange
            string serviceName = "serviceName";

            // Act
            var result = ActorNameFormat.GetFabricServiceName(typeof(object), serviceName);

            // Assert
            result.Should().Be(serviceName);
        }
    }
}