using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    sealed class TestActor : Actor, ITestActor
    {
        public TestActor(ActorService actorService, ActorId actorId) : base(actorService, actorId) { }

        public Task TestMethod()
        {
            return Task.CompletedTask;
        }
    }

    internal interface ITestActor : IActor
    {
        public Task TestMethod();
    }
}
