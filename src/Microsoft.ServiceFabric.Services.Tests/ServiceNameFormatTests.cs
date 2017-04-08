using Xunit;

namespace Microsoft.ServiceFabric.Services.Tests
{
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