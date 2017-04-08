using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests
{
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