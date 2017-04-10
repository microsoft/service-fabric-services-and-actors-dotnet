// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    using System;
    using Xunit;

    public class RemoteExceptionInformationTests
    {
        [Fact]
        public void FromExceptionTest()
        {
            // Arrange
            var expectedLength = 944;
            Exception exception = new Exception("Test Exception");

            // Act
            var result = RemoteExceptionInformation.FromException(exception);

            // Assert
            Assert.Equal(expectedLength, result.Data.Length);
        }
    }
}