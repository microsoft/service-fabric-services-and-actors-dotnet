// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Numerics;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public class ServiceMeterProviderTest
    {
        const string TestNodeName = "TestNodeName";
        const string TestServiceTypeName = "TestServiceType";
        const string TestServiceUriString = "fabric:/TestApplication/TestService";
        const string TestApplicationName = "TestApplicationName";
        const string TestApplicationTypeName = "TestApplicationTypeName";
        const long ReplicaId = 1L;

        readonly Guid testPartitionId = new Guid();
        readonly ServiceContext serviceContext;

        public ServiceMeterProviderTest()
        {
            var codePackageActivationContext = Mock.Of<ICodePackageActivationContext>();
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationName).Returns(TestApplicationName);
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationTypeName).Returns(TestApplicationTypeName);

            this.serviceContext = new TestServiceContext(
                new NodeContext(TestNodeName, new NodeId(BigInteger.Zero, BigInteger.Zero), BigInteger.Zero, string.Empty, string.Empty),
                codePackageActivationContext,
                TestServiceTypeName,
                new Uri(TestServiceUriString),
                null,
                testPartitionId,
                ReplicaId);
        }

        public class Constructor : ServiceMeterProviderTest
        {
            [Fact]
            public void ShouldRecordRequiredDimensionsFromServiceContext()
            {
                var sut = new TestMeterProvider<int>(serviceContext);

                var systemDimensions = sut.Field<IDictionary<string, string>>().Value;

                Assert.Equal(ReplicaId.ToString(), systemDimensions["ReplicaOrInstanceId"]);
                Assert.Equal(testPartitionId.ToString(), systemDimensions["PartitionId"]);
                Assert.Equal(TestServiceTypeName, systemDimensions["ServiceTypeName"]);
                Assert.Equal(TestServiceUriString, systemDimensions["ServiceName"]);
                Assert.Equal(TestApplicationName, systemDimensions["ApplicationName"]);
                Assert.Equal(TestApplicationTypeName, systemDimensions["ApplicationTypeName"]);
            }

            [Fact]
            public void ShouldNotRecordRequiredDimensionsFromNullServiceContext()
            {
                Assert.Throws<ArgumentNullException>(() => new TestMeterProvider<int>(null));
            }
        }

        private class TestMeterProvider<TValueType> : ServiceMeterProvider<TValueType>
        {
            public TestMeterProvider(ServiceContext serviceContext)
                : base(serviceContext)
            {
            }

            public override IMeter<TValueType> CreateMeter(string metricNamespace, string name)
            {
                throw new NotImplementedException();
            }

            public override IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name)
            {
                throw new NotImplementedException();
            }

            public override IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name)
            {
                throw new NotImplementedException();
            }

            public override IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name)
            {
                throw new NotImplementedException();
            }
        }

        private class TestServiceContext : ServiceContext
        {
            public TestServiceContext(NodeContext nodeContext, ICodePackageActivationContext codePackageActivationContext, string serviceTypeName, Uri serviceName, byte[] initializationData, Guid partitionId, long replicaId)
                : base(nodeContext, codePackageActivationContext, serviceTypeName, serviceName, initializationData, partitionId, replicaId)
            {
            }
        }
    }
}
