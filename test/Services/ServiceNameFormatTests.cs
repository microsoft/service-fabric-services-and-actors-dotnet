// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using Xunit;

    /// <summary>
    /// Test class for ServiceNameFormat.
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
            Assert.Equal(serviceName, result);
        }
    }
}
