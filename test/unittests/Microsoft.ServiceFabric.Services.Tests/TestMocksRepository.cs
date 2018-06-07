// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
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
        private static Guid mockPartitionID = Guid.NewGuid();
        private static long mockReplicaOrInstanceID = 99999999999;
        private static string mockServiceTypeName = "MockServiceTypeName";
        private static Uri mockServiceUri = new Uri("fabric:/MockServiceName");

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
    }
}
