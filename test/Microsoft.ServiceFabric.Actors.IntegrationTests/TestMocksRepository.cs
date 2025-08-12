// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Numerics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Moq;

    /// <summary>
    /// Contains mocks needed by Aspnet core listener unit tests.
    /// </summary>
    internal static class TestMocksRepository
    {
        // For Service Context
        private static Guid mockPartitionID = Guid.NewGuid();
        private static long mockReplicaOrInstanceID = 99999999999;
        private static Uri mockServiceUri = new Uri("fabric:/MockServiceName");
        private static string mockServiceTypeName = "MockServiceTypeName";

        // For NodeContext
        private static string mockFQDN = "MockFQDN";
        private static string mockNodeName = "MockNodeName";
        private static string mockNodeType = "MockNodeType";

        // For CodePackageActiviationContext
        private static string mockWorkDirectory = "MockWorkDirectory";
        private static string mockLogDirectory = "MockLogDirectory";
        private static string mockTempDirectory = "MockTempDirectory";
        private static string mockContextId = "MockContextId";
        private static string mockCodePackageName = "MockCodePackageName";
        private static string mockCodePackageVersion = "MockCodePackageVersion";
        private static string mockApplciationName = "MockApplicationName";
        private static string mockApplicationTypeName = "MockApplicationTypeName";

        /// <summary>
        /// Gets a mock StatefulServiceContext.
        /// </summary>
        /// <returns>A mock StatefulServiceContext.</returns>
        internal static StatefulServiceContext GetMockStatefulServiceContext()
        {
            return new StatefulServiceContext(
                GetNodeContext(),
                GetCodePackageActivationContext(),
                mockServiceTypeName,
                mockServiceUri,
                null,
                mockPartitionID,
                mockReplicaOrInstanceID);
        }

        /// <summary>
        /// Geta a mock StatelessServiceContext.
        /// </summary>
        /// <returns>A mock StatelessServiceContext.</returns>
        internal static StatelessServiceContext GetMockStatelessServiceContext()
        {
            return new StatelessServiceContext(
                GetNodeContext(),
                GetCodePackageActivationContext(),
                mockServiceTypeName,
                mockServiceUri,
                null,
                mockPartitionID,
                mockReplicaOrInstanceID);
        }

        /// <summary>
        /// Gets mock Actor Service.
        /// </summary>
        /// <typeparam name="T">Type of Actor.</typeparam>
        /// <returns>Actor Service.</returns>
        internal async static Task<ActorService> GetActorService<T>(ActorServiceSettings actorServiceSettings = null)
            where T : Actor
        {
            ActorService actorService = new ActorService(
                GetMockStatefulServiceContext(),
                ActorTypeInformation.Get(typeof(T)),
                null,
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
        /// Gets a mock ActorTimer.
        /// </summary>
        /// <returns>A mock ActorTime.</returns>
        internal static IActorTimer GetMockActorTimer()
        {
            // Create mock StateProvider and setup required things needed by tests.
            var mockTimer = new Mock<IActorTimer>();
            mockTimer.SetupAllProperties();
            return mockTimer.Object;
        }

        internal static ICodePackageActivationContext GetCodePackageActivationContext()
        {
            // Create mock Context and setup required things needed by tests.
            var mockContext = new Mock<ICodePackageActivationContext>();
            mockContext.SetupAllProperties();
            mockContext.Setup(x => x.WorkDirectory).Returns(mockWorkDirectory);
            mockContext.Setup(x => x.LogDirectory).Returns(mockLogDirectory);
            mockContext.Setup(x => x.TempDirectory).Returns(mockTempDirectory);
            mockContext.Setup(x => x.ContextId).Returns(mockContextId);
            mockContext.Setup(x => x.CodePackageName).Returns(mockCodePackageName);
            mockContext.Setup(x => x.CodePackageVersion).Returns(mockCodePackageVersion);
            mockContext.Setup(x => x.ApplicationName).Returns(mockApplciationName);
            mockContext.Setup(x => x.ApplicationTypeName).Returns(mockApplicationTypeName);
            return mockContext.Object;
        }

        private static NodeContext GetNodeContext()
        {
            return new NodeContext(
                mockNodeName,
                new NodeId(BigInteger.Zero, BigInteger.Zero),
                BigInteger.Zero,
                mockNodeType,
                mockFQDN);
        }
    }
}
