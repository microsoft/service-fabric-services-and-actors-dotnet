// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Xunit;

    /// <summary>
    /// Class for RemoteExceptionInformation tests.
    /// </summary>
    public class RemoteExceptionInformationTests
    {
        /// <summary>
        /// Tests RemoteExceptionInformtaion created from a test exception.
        /// </summary>
        [Fact]
        public void FromExceptionTest()
        {
            // Arrange
            var expectedLength = 944;
            var exception = new Exception("Test Exception");

            // Act
            var result = RemoteExceptionInformation.FromException(exception);

            // Assert
            result.Data.Length.Should().Be(expectedLength);
        }
    }
}
