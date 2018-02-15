// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Fabric;
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
        internal static Guid MockPartitionID = Guid.NewGuid();
        internal static long MockReplicaOrInstanceID = 99999999999;
        internal static Uri MockServiceUri = new Uri("fabric:/MockServiceName");
        internal static string MockServiceTypeName = "MockServiceTypeName";

        // For NodeContext
        internal static string MockFQDN = "MockFQDN";
        internal static string MockNodeName = "MockNodeName";
        internal static string MockNodeType = "MockNodeType";

        // For CodePackageActiviationContext
        internal static string MockWorkDirectory = "MockWorkDirectory";
        internal static string MockLogDirectory = "MockLogDirectory";
        internal static string MockTempDirectory = "MockTempDirectory";
        internal static string MockContextId = "MockContextId";
        internal static string MockCodePackageName = "MockCodePackageName";
        internal static string MockCodePackageVersion = "MockCodePackageVersion";
        internal static string MockApplciationName = "MockApplicationName";
        internal static string MockApplicationTypeName = "MockApplicationTypeName";

        internal static StatefulServiceContext GetMockStatefulServiceContext()
        {
            return new StatefulServiceContext(GetNodeContext(),
                GetCodePackageActivationContext(),
                MockServiceTypeName,
                MockServiceUri,
                null,
                MockPartitionID,
                MockReplicaOrInstanceID);
        }

        internal static StatelessServiceContext GetMockStatelessServiceContext()
        {
            return new StatelessServiceContext(GetNodeContext(),
                GetCodePackageActivationContext(),
                MockServiceTypeName,
                MockServiceUri,
                null,
                MockPartitionID,
                MockReplicaOrInstanceID);
        }

        internal static ActorService GetActorService<T>() where T : Actor
        {
            return new ActorService(
                GetMockStatefulServiceContext(),
                ActorTypeInformation.Get(typeof(T)),
                null,
                null,
                GetMockActorStateProvider());
        }

        internal static IActorTimer GetMockActorTimer()
        {
            // Create mock StateProvider and setup required things needed by tests.
            var mockTimer = new Mock<IActorTimer>();
            mockTimer.SetupAllProperties();
            return mockTimer.Object;
        }

        private static NodeContext GetNodeContext()
        {
            return new NodeContext(MockNodeName,
                new NodeId(BigInteger.Zero, BigInteger.Zero),
                BigInteger.Zero,
                MockNodeType,
                MockFQDN);
        }

        private static ICodePackageActivationContext GetCodePackageActivationContext()
        {
            // Create mock Context and setup required things needed by tests.
            var mockContext = new Mock<ICodePackageActivationContext>();
            mockContext.SetupAllProperties();
            mockContext.Setup(x => x.WorkDirectory).Returns(MockWorkDirectory);
            mockContext.Setup(x => x.LogDirectory).Returns(MockLogDirectory);
            mockContext.Setup(x => x.TempDirectory).Returns(MockTempDirectory);
            mockContext.Setup(x => x.ContextId).Returns(MockContextId);
            mockContext.Setup(x => x.CodePackageName).Returns(MockCodePackageName);
            mockContext.Setup(x => x.CodePackageVersion).Returns(MockCodePackageVersion);
            mockContext.Setup(x => x.ApplicationName).Returns(MockApplciationName);
            mockContext.Setup(x => x.ApplicationTypeName).Returns(MockApplicationTypeName);
            return mockContext.Object;
        }

        internal static IActorStateProvider GetMockActorStateProvider()
        {
            // Create mock StateProvider and setup required things needed by tests.
            var mockStateProvider = new Mock<IActorStateProvider>();
            mockStateProvider.SetupAllProperties();

            // for IActorStateProvider
            mockStateProvider.Setup(
                x =>
                    x.SaveStateAsync(It.IsAny<ActorId>(), It.IsAny<IReadOnlyCollection<ActorStateChange>>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            mockStateProvider.Setup(
                x =>
                    x.ContainsStateAsync(It.IsAny<ActorId>(), It.IsAny<string>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));

            mockStateProvider.Setup(
                x =>
                    x.RemoveActorAsync(It.IsAny<ActorId>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            mockStateProvider.Setup(
                x =>
                    x.SaveReminderAsync(It.IsAny<ActorId>(), It.IsAny<IActorReminder>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            mockStateProvider.Setup(
                x =>
                    x.DeleteReminderAsync(It.IsAny<ActorId>(), It.IsAny<string>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            mockStateProvider.Setup(
                x =>
                    x.EnumerateStateNamesAsync(It.IsAny<ActorId>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new List<string>() as IEnumerable<string>));

            return mockStateProvider.Object;
        }
    }
}
