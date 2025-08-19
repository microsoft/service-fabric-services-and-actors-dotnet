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
using Microsoft.ServiceFabric.Diagnostics.Metrics.Interop;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public class Int64MeterProviderTest
    {
        static readonly protected IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly string testNodeName = fuzzy.String();
        readonly string testServiceTypeName = fuzzy.String();
        readonly string testServiceUriString = "fabric:/TestApplication/TestService";
        readonly string testApplicationName = fuzzy.String();
        readonly string testApplicationTypeName = fuzzy.String();
        readonly long replicaId = fuzzy.Int64();
        readonly Guid testPartitionId = new Guid();

        readonly protected ServiceContext serviceContext;

        public Int64MeterProviderTest()
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

            typeof(ServiceMeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(() => Mock.Of<IFabricMeterProvider>());
            typeof(ServiceMeterProvider<long>).Field<Func<IFabricMeterProvider, string, string, string[], uint, IFabricMeter>>().Set((meterProvider, metricNamespace, metricName, dimensionNames, dimensionCount) => Mock.Of<IFabricMeter>());
        }

        public class CreateMeter : Int64MeterProviderTest
        {
            readonly string testNamespace = fuzzy.String();
            readonly string testMetric = fuzzy.String();
            readonly string testDimension1 = fuzzy.String();
            readonly string testDimension2 = fuzzy.String();
            readonly string testDimension3 = fuzzy.String();
            readonly Int64MeterProvider sut;

            public CreateMeter()
            {
                sut = new Int64MeterProvider(serviceContext);
            }

            [Fact]
            public void ShouldCreateMeterWithCorrectSystemDimensions()
            {
                var meter = sut.CreateMeter(testNamespace, testMetric);

                Assert.NotNull(meter);
                Assert.IsType<Int64Meter>(meter);

                var expectedSystemDimensions = sut.Protected().Field<IList<string>>().Value;
                Assert.Equal(expectedSystemDimensions, ((Int64Meter)meter).Field<IList<string>>().Value);
            }

            [Fact]
            public void ShouldCreateMeter1DWithCorrectSystemDimensions()
            {
                var meter1D = sut.CreateMeter(testNamespace, testMetric, testDimension1);

                Assert.NotNull(meter1D);
                Assert.IsType<Int64Meter1D>(meter1D);

                var expectedSystemDimensions = sut.Protected().Field<IList<string>>().Value;
                expectedSystemDimensions.Add(testDimension1);
                Assert.Equal(expectedSystemDimensions, ((Int64Meter1D)meter1D).Field<IList<string>>().Value);
            }

            [Fact]
            public void ShouldCreateMeter2DWithCorrectSystemDimensions()
            {
                var meter2D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2);

                Assert.NotNull(meter2D);
                Assert.IsType<Int64Meter2D>(meter2D);

                var expectedSystemDimensions = sut.Protected().Field<IList<string>>().Value;
                expectedSystemDimensions.Add(testDimension1);
                expectedSystemDimensions.Add(testDimension2);
                Assert.Equal(expectedSystemDimensions, ((Int64Meter2D)meter2D).Field<IList<string>>().Value);
            }

            [Fact]
            public void ShouldCreateMeter3DWithCorrectSystemDimensions()
            {
                var meter3D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2, testDimension3);
                Assert.NotNull(meter3D);
                Assert.IsType<Int64Meter3D>(meter3D);
                var expectedSystemDimensions = sut.Protected().Field<IList<string>>().Value;
                expectedSystemDimensions.Add(testDimension1);
                expectedSystemDimensions.Add(testDimension2);
                expectedSystemDimensions.Add(testDimension3);
                Assert.Equal(expectedSystemDimensions, ((Int64Meter3D)meter3D).Field<IList<string>>().Value);
            }
        }
    }
}
