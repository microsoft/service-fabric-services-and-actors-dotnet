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

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    public class ConfigurableMeterProviderTest
    {
        const string TestNodeName = "TestNodeName";
        const string TestNodeVersion = "12.0.0";
        const string TestServiceTypeName = "TestServiceType";
        const string TestServiceUriString = "fabric:/TestApplication/TestService";
        const string TestApplicationName = "TestAplicationName";
        const string TestApplicationTypeName = "TestAplicationTypeName";

        readonly Guid testPartitionId = new Guid();
        readonly ConfigStoreWrapper configStoreWrapped = Mock.Of<ConfigStoreWrapper>();
        readonly ServiceContext serviceContext;

        public ConfigurableMeterProviderTest()
        {
            Mock.Get(configStoreWrapped).Setup(x => x.ReadConfig("FabricNode", "InstanceName")).Returns(TestNodeName);
            Mock.Get(configStoreWrapped).Setup(x => x.ReadConfig("FabricNode", "NodeVersion")).Returns(TestNodeVersion + ":0:0");
            Mock.Get(configStoreWrapped).Setup(x => x.ReadConfig("Telemetry/Metrics", "IsEnabled")).Returns(true.ToString);

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
                1L);
        }

        public class Constructor : ConfigurableMeterProviderTest
        {
            [Fact]
            public void ShouldThrowExceptionOnConfigStoreNull()
            {
                Assert.Throws<ArgumentNullException>(() => new TestMeterProvider<int>(null, null));
            }

            [Fact]
            public void ShouldRecordRequiredDimensionsFromConfigStore()
            {
                var sut = new TestMeterProvider<int>(configStoreWrapped, null);

                var systemDimensions = sut.Field<IDictionary<string, string>>().Value;
                var metricsEnabled = sut.Field<bool>().Value;

                Assert.Equal(TestNodeName, systemDimensions["NodeName"]);
                Assert.Equal(TestNodeVersion, systemDimensions["RuntimeVersion"]);
                Assert.True(metricsEnabled);
            }

            [Fact]
            public void ShouldRecordRequiredDimensionsFromServiceContext()
            {
                var sut = new TestMeterProvider<int>(configStoreWrapped, serviceContext);

                var systemDimensions = sut.Field<IDictionary<string, string>>().Value;

                Assert.Equal("1", systemDimensions["ReplicaOrInstanceId"]);
                Assert.Equal(testPartitionId.ToString(), systemDimensions["PartitionId"]);
                Assert.Equal(TestServiceTypeName, systemDimensions["ServiceTypeName"]);
                Assert.Equal(TestServiceUriString, systemDimensions["ServiceName"]);
                Assert.Equal(TestApplicationName, systemDimensions["ApplicationName"]);
                Assert.Equal(TestApplicationTypeName, systemDimensions["ApplicationTypeName"]);
            }

            [Fact]
            public void ShouldNotRecordRequiredDimensionsFromNullServiceContext()
            {
                var sut = new TestMeterProvider<int>(configStoreWrapped, null);

                var systemDimensions = sut.Field<IDictionary<string, string>>().Value;

                Assert.False(systemDimensions.ContainsKey("ReplicaOrInstanceId"));
                Assert.False(systemDimensions.ContainsKey("PartitionId"));
                Assert.False(systemDimensions.ContainsKey("ServiceTypeName"));
                Assert.False(systemDimensions.ContainsKey("ServiceName"));
                Assert.False(systemDimensions.ContainsKey("ApplicationName"));
                Assert.False(systemDimensions.ContainsKey("ApplicationTypeName"));
            }
        }

        private class TestMeterProvider<ValueType> : ConfigurableMeterProvider<ValueType>
        {
            public TestMeterProvider(ConfigStoreWrapper configStoreWrapped, ServiceContext serviceContext)
                : base(configStoreWrapped, serviceContext)
            {
            }

            public override IMeter<ValueType> CreateMeter(string name, string metricNamespace = "")
            {
                throw new NotImplementedException();
            }

            public override IMeter1D<ValueType> CreateMeter(string name, string dimension1Name, string metricNamespace = "")
            {
                throw new NotImplementedException();
            }

            public override IMeter2D<ValueType> CreateMeter(string name, string dimension1Name, string dimension2Name, string metricNamespace = "")
            {
                throw new NotImplementedException();
            }

            public override IMeter3D<ValueType> CreateMeter(string name, string dimension1Name, string dimension2Name, string dimension3Name, string metricNamespace = "")
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
