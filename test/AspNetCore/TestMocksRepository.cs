// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Fabric.Description;
using System.Numerics;
using Moq;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Contains mocks needed by Aspnet core listener unit tests.
    /// </summary>
    internal static class TestMocksRepository
    {
        private const long MockReplicaOrInstanceID = 99999999999;
        private const string MockFQDN = "MockFQDN";
        private const string MockServiceTypeName = "MockServiceTypeName";
        private const string MockServiceUri = "fabric:/MockServiceName";
        private static Guid mockPartitionID = Guid.NewGuid();

        internal static StatefulServiceContext GetMockStatefulServiceContext()
        {
            return new StatefulServiceContext(
                GetNodeContext(),
                GetCodePackageActivationContext(),
                MockServiceTypeName,
                new Uri(MockServiceUri),
                null,
                mockPartitionID,
                MockReplicaOrInstanceID);
        }

        internal static StatelessServiceContext GetMockStatelessServiceContext()
        {
            return new StatelessServiceContext(
                GetNodeContext(),
                GetCodePackageActivationContext(),
                MockServiceTypeName,
                new Uri(MockServiceUri),
                null,
                mockPartitionID,
                MockReplicaOrInstanceID);
        }

        private static NodeContext GetNodeContext()
        {
            return new NodeContext(
                "MockNode",
                new NodeId(BigInteger.Zero, BigInteger.Zero),
                BigInteger.Zero,
                "MockNodeType",
                MockFQDN);
        }

        private static ICodePackageActivationContext GetCodePackageActivationContext()
        {
            var endpointCollection = new KeyedCollectionImpl<string, EndpointResourceDescription>(x => x.Name);

            // Create mock Context and setup required things needed by tests.
            var mockContext = new Mock<ICodePackageActivationContext>();
            mockContext.Setup(x => x.GetEndpoints()).Returns(endpointCollection);
            mockContext.Setup(y => y.GetEndpoint(It.IsAny<string>())).Returns<string>(name =>
            {
                return endpointCollection[name];
            });

            return mockContext.Object;
        }
    }
}
