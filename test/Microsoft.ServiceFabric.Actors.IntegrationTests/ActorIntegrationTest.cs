using System;
using System.Fabric;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Moq;

namespace Microsoft.ServiceFabric.Actors.IntegrationTests
{
    public abstract class ActorIntegrationTest
    {
        /// <summary>
        /// Gets mock Actor Service.
        /// </summary>
        /// <typeparam name="T">Type of Actor.</typeparam>
        /// <returns>Actor Service.</returns>
        protected async Task<ActorService> GetActorService<T>(
            Func<ActorService, ActorId, ActorBase> actorFactory = null,
            ActorServiceSettings actorServiceSettings = null)
            where T : Actor
        {
            ActorService actorService = new ActorService(
                GetMockStatefulServiceContext(),
                ActorTypeInformation.Get(typeof(T)),
                actorFactory,
                null,
                new NullActorStateProvider(),
                actorServiceSettings);

            IStatefulUserServiceReplica statefulServiceReplica = actorService;
            await statefulServiceReplica.OnOpenAsync(ReplicaOpenMode.New, CancellationToken.None);
            await statefulServiceReplica.OnChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None);
            await statefulServiceReplica.RunAsync(CancellationToken.None);

            return actorService;
        }

        /// <summary>
        /// Gets a mock StatefulServiceContext.
        /// </summary>
        /// <returns>A mock StatefulServiceContext.</returns>
        protected StatefulServiceContext GetMockStatefulServiceContext()
        {
            return new StatefulServiceContext(
                GetNodeContext(),
                GetCodePackageActivationContext(),
                "MockServiceTypeName",
                new Uri("fabric:/MockServiceName"),
                null,
                new Guid(),
                1);
        }

        /// <summary>
        /// Gets a mock ActorTimer.
        /// </summary>
        /// <returns>A mock ActorTime.</returns>
        protected IActorTimer GetMockActorTimer()
        {
            // Create mock StateProvider and setup required things needed by tests.
            var mockTimer = new Mock<IActorTimer>();
            mockTimer.SetupAllProperties();
            return mockTimer.Object;
        }

        protected ICodePackageActivationContext GetCodePackageActivationContext()
        {
            // Create mock Context and setup required things needed by tests.
            var mockContext = new Mock<ICodePackageActivationContext>();
            mockContext.SetupAllProperties();
            mockContext.Setup(x => x.WorkDirectory).Returns("MockWorkDirectory");
            mockContext.Setup(x => x.LogDirectory).Returns("MockLogDirectory");
            mockContext.Setup(x => x.TempDirectory).Returns("MockTempDirectory");
            mockContext.Setup(x => x.ContextId).Returns("MockContextId");
            mockContext.Setup(x => x.CodePackageName).Returns("MockCodePackageName");
            mockContext.Setup(x => x.CodePackageVersion).Returns("MockCodePackageVersion");
            mockContext.Setup(x => x.ApplicationName).Returns("MockApplicationName");
            mockContext.Setup(x => x.ApplicationTypeName).Returns("MockApplicationTypeName");
            return mockContext.Object;
        }

        protected NodeContext GetNodeContext()
        {
            return new NodeContext(
                "MockNodeName",
                new NodeId(BigInteger.Zero, BigInteger.Zero),
                BigInteger.Zero,
                "MockNodeType",
                "MockFQDN");
        }
    }
}

