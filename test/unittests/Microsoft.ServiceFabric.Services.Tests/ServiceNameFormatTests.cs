// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Test class for ServiceNameForamt.
    /// </summary>
    public class ServiceNameFormatTests
    {
        /// <summary>
        /// Tests ServiceNameFormat.GetEndpointName.
        /// </summary>
        [Fact]
        public void GetServiceNameFormat_NoServiceNameProvided_ReturnEndpointName()
        {
            // Arrange
            var serviceName = "ObjectServiceEndpoint";

            // Act
            var result = ServiceNameFormat.GetEndpointName(typeof(object));

            // Assert
            result.Should().Be(serviceName);
        }
    }
}
