// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Numerics;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public class ServiceMeterProviderTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly string testNodeName = fuzzy.String();
        readonly string testServiceTypeName = fuzzy.String();
        readonly string testServiceUriString = "fabric:/TestApplication/TestService";
        readonly string testApplicationName = fuzzy.String();
        readonly string testApplicationTypeName = fuzzy.String();
        readonly long replicaId = fuzzy.Int64();
        readonly Guid testPartitionId = new Guid();

        readonly ServiceContext serviceContext;

        public ServiceMeterProviderTest()
        {
            var codePackageActivationContext = Mock.Of<ICodePackageActivationContext>();
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationName).Returns(testApplicationName);
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationTypeName).Returns(testApplicationTypeName);

            this.serviceContext = new TestServiceContext(
                new NodeContext(testNodeName, new NodeId(BigInteger.Zero, BigInteger.Zero), BigInteger.Zero, string.Empty, string.Empty),
                codePackageActivationContext,
                testServiceTypeName,
                new Uri(testServiceUriString),
                null,
                testPartitionId,
                replicaId);

            typeof(ServiceMeterProvider<int>).Field<Func<IFabricMeterProvider>>().Set(() => Mock.Of<IFabricMeterProvider>());
        }

        public class Constructor : ServiceMeterProviderTest
        {
            [Fact]
            public void ShouldRecordRequiredDimensionsFromServiceContext()
            {
                var sut = new TestMeterProvider<int>(serviceContext);

                var systemDimensions = sut.Protected().Field<IList<string>>().Value;

                Assert.Equal(replicaId.ToString(), systemDimensions[0]);
                Assert.Equal(testPartitionId.ToString(), systemDimensions[1]);
                Assert.Equal(testServiceTypeName, systemDimensions[2]);
                Assert.Equal(testServiceUriString, systemDimensions[3]);
                Assert.Equal(testApplicationName, systemDimensions[4]);
                Assert.Equal(testApplicationTypeName, systemDimensions[5]);
            }

            [Fact]
            public void ShouldNotRecordRequiredDimensionsFromNullServiceContext()
            {
                Assert.Throws<ArgumentNullException>(() => new TestMeterProvider<int>(null));
            }
        }

        class TestMeterProvider<TValueType> : ServiceMeterProvider<TValueType>
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
    }
}
