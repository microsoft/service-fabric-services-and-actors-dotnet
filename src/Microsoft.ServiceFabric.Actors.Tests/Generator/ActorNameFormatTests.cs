using Microsoft.ServiceFabric.Actors.Generator;
using Xunit;

namespace Microsoft.ServiceFabric.ActorsTests.Generator
{
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