// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Numerics;
    using System.Threading;
    using System.Threading.Tasks;
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
        internal static ActorService GetActorService<T>()
            where T : Actor
        {
            return new ActorService(
                GetMockStatefulServiceContext(),
                ActorTypeInformation.Get(typeof(T)),
                null,
                null,
                GetMockActorStateProvider());
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

        /// <summary>
        /// Gets a mock ActorStateProvider.
        /// </summary>
        /// <returns>Mock ActorStateProvider.</returns>
        internal static IActorStateProvider GetMockActorStateProvider()
        {
            // Create mock StateProvider and setup required things needed by tests.
            var mockStateProvider = new Mock<IActorStateProvider>();
            mockStateProvider.SetupAllProperties();

            // for IActorStateProvider
            mockStateProvider.Setup(
                x =>
                    x.SaveStateAsync(
                        It.IsAny<ActorId>(),
                        It.IsAny<IReadOnlyCollection<ActorStateChange>>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            mockStateProvider.Setup(
                x =>
                    x.ContainsStateAsync(
                        It.IsAny<ActorId>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));

            mockStateProvider.Setup(
                x =>
                    x.RemoveActorAsync(
                        It.IsAny<ActorId>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            mockStateProvider.Setup(
                x =>
                    x.SaveReminderAsync(
                        It.IsAny<ActorId>(),
                        It.IsAny<IActorReminder>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            mockStateProvider.Setup(
                x =>
                    x.DeleteReminderAsync(
                        It.IsAny<ActorId>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            mockStateProvider.Setup(
                x =>
                    x.EnumerateStateNamesAsync(It.IsAny<ActorId>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new List<string>() as IEnumerable<string>));

            return mockStateProvider.Object;
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

        private static ICodePackageActivationContext GetCodePackageActivationContext()
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
    }
}
