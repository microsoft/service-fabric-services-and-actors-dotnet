// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Tests
{
    using Xunit;

    public class ServiceNameFormatTests
    {
        [Fact]
        public void GetServiceNameFormat_NoServiceNameProvided_ReturnEndpointName()
        {
            // Arrange
            string serviceName = "ObjectServiceEndpoint";

            // Act
            var result = ServiceNameFormat.GetEndpointName(typeof(object));

            // Assert
            Assert.Equal(serviceName, result);
        }
    }
}