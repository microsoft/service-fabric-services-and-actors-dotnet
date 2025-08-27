// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public class Int64MeterProviderTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly string testNodeName = fuzzy.String();
        readonly string testServiceTypeName = fuzzy.String();
        readonly Uri testServiceNameUri = fuzzy.Uri();
        readonly string testApplicationName = fuzzy.String();
        readonly string testApplicationTypeName = fuzzy.String();
        readonly long replicaId = fuzzy.Int64();
        readonly Guid testPartitionId = Guid.NewGuid();

        readonly ServiceContext serviceContext;

        public Int64MeterProviderTest()
        {
            var codePackageActivationContext = Mock.Of<ICodePackageActivationContext>();
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationName).Returns(testApplicationName);
            Mock.Get(codePackageActivationContext).SetupGet(x => x.ApplicationTypeName).Returns(testApplicationTypeName);
            this.serviceContext = new Mock<ServiceContext>(fuzzy.NodeContext(), codePackageActivationContext, testServiceTypeName, testServiceNameUri, fuzzy.Array(fuzzy.Byte), testPartitionId, replicaId).Object;

            typeof(ServiceMeterProvider<long>).Field<Func<IFabricMeterProvider>>().Set(() => Mock.Of<IFabricMeterProvider>());
        }

        public class CreateMeter : Int64MeterProviderTest
        {
            readonly string testNamespace = fuzzy.String();
            readonly string testMetric = fuzzy.String();
            readonly string testDimension1 = fuzzy.String();
            readonly string testDimension2 = fuzzy.String();
            readonly string testDimension3 = fuzzy.String();

            readonly Int64MeterProvider sut;
            readonly IList<string> systemDimensionsNames;
            readonly IList<string> systemDimensionsValues;
            readonly IFabricMeterProvider fabricMeterProvider = new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object;

            public CreateMeter()
            {
                sut = new Int64MeterProvider(serviceContext);
                systemDimensionsNames = sut.Private().Field<IList<string>>().Value;
                systemDimensionsValues = sut.Protected().Field<IList<string>>().Value;
                sut.Field<IFabricMeterProvider>().Set(fabricMeterProvider);
            }

            [Fact]
            public void CreatesMeterWithCorrectSystemDimensions()
            {
                IMeter<long> meter = sut.CreateMeter(testNamespace, testMetric);

                Assert.NotNull(meter);
                Assert.IsType<Int64Meter>(meter);

                Assert.Equal(systemDimensionsValues, ((Int64Meter)meter).Field<IEnumerable<string>>().Value);
            }

            [Fact]
            public void CreatesMeter1DWithCorrectSystemDimensions()
            {
                IMeter1D<long> meter1D = sut.CreateMeter(testNamespace, testMetric, testDimension1);

                Assert.NotNull(meter1D);
                Assert.IsType<Int64Meter1D>(meter1D);

                Assert.Equal(systemDimensionsValues, ((Int64Meter1D)meter1D).Field<IEnumerable<string>>().Value);
            }

            [Fact]
            public void CreatesMeter2DWithCorrectSystemDimensions()
            {
                IMeter2D<long> meter2D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2);

                Assert.NotNull(meter2D);
                Assert.IsType<Int64Meter2D>(meter2D);

                Assert.Equal(systemDimensionsValues, ((Int64Meter2D)meter2D).Field<IEnumerable<string>>().Value);
            }

            [Fact]
            public void CreatesMeter3DWithCorrectSystemDimensions()
            {
                IMeter3D<long> meter3D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2, testDimension3);
                Assert.NotNull(meter3D);
                Assert.IsType<Int64Meter3D>(meter3D);

                Assert.Equal(systemDimensionsValues, ((Int64Meter3D)meter3D).Field<IEnumerable<string>>().Value);
            }

            [Fact]
            public void CreatesNativeMeterWithCorrectDimensions()
            {
                IMeter<long> meter = sut.CreateMeter(testNamespace, testMetric);

                var combinedDimensions = systemDimensionsNames.ToArray();
                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensions.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensions))), Times.Once);
            }

            [Fact]
            public void CreatesNativeMeterWithCorrectDimensions1D()
            {
                IMeter1D<long> meter1D = sut.CreateMeter(testNamespace, testMetric, testDimension1);

                var combinedDimensions = new List<string>(systemDimensionsNames) { testDimension1 }.ToArray();
                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensions.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensions))), Times.Once);
            }

            [Fact]
            public void CreatesNativeMeterWithCorrectDimensions2D()
            {
                IMeter2D<long> meter2D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2);

                var combinedDimensions = new List<string>(systemDimensionsNames) { testDimension1, testDimension2 }.ToArray();
                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensions.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensions))), Times.Once);
            }

            [Fact]
            public void CreatesNativeMeterWithCorrectDimensions3D()
            {
                IMeter3D<long> meter3D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2, testDimension3);

                var combinedDimensions = new List<string>(systemDimensionsNames) { testDimension1, testDimension2, testDimension3 }.ToArray();
                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensions.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensions))), Times.Once);
            }
        }
    }
}
