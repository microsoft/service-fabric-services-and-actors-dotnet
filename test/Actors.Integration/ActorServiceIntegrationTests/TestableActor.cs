using Microsoft.ServiceFabric.Actors.Runtime;

namespace Microsoft.ServiceFabric.Actors.ActorServiceIntegrationTests
{
        interface ITestableActor : IActor
        { }

        class TestActor : Actor, ITestableActor
        {
            public TestActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
            {
            }
        }
}