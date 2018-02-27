// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using System;
    using System.Fabric;
    using System.Numerics;
    using Moq;

    /// <summary>
    /// Contains mocks needed by tests for Microsoft.ServiceFabric.Services
    /// </summary>
    internal static class TestMocksRepository
    {
        internal static Guid MockPartitionID = Guid.NewGuid();
        internal static long MockReplicaOrInstanceID = 99999999999;
        internal static string MockFQDN = "MockFQDN";
        internal static string MockServiceTypeName = "MockServiceTypeName";
        internal static Uri MockServiceUri = new Uri("fabric:/MockServiceName");

        internal static StatefulServiceContext GetMockStatefulServiceContext()
        {
            return new StatefulServiceContext(
                GetNodeContext(),
                GetCodePackageActivationContext(),
                MockServiceTypeName,
                MockServiceUri,
                null,
                MockPartitionID,
                MockReplicaOrInstanceID);
        }

        internal static StatelessServiceContext GetMockStatelessServiceContext()
        {
            return new StatelessServiceContext(
                GetNodeContext(),
                GetCodePackageActivationContext(),
                MockServiceTypeName,
                MockServiceUri,
                null,
                MockPartitionID,
                MockReplicaOrInstanceID);
        }
        public static NodeContext GetNodeContext()
        {
            return new NodeContext(
                "MockNode",
                new NodeId(BigInteger.Zero, BigInteger.Zero),
                BigInteger.Zero,
                "MockNodeType",
                "MockFQDN");
        }

        public static ICodePackageActivationContext GetCodePackageActivationContext()
        {
            // Create mock Context and setup required things needed by tests.
            var mockContext = new Mock<ICodePackageActivationContext>();
            return mockContext.Object;
        }
    }
}
