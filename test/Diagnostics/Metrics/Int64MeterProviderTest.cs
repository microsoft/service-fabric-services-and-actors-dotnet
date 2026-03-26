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
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public class Int64MeterProviderTest : FabricTelemetryDllFixture
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly ServiceContext serviceContext = fuzzy.ServiceContext();

        public class CreateMeter : Int64MeterProviderTest
        {
            readonly string testNamespace = fuzzy.String();
            readonly string testMetric = fuzzy.String();
            readonly string testDimension1 = fuzzy.String();
            readonly string testDimension2 = fuzzy.String();
            readonly string testDimension3 = fuzzy.String();

            readonly Int64MeterProvider sut;
            readonly IReadOnlyCollection<string> systemDimensionsNames;
            readonly IReadOnlyCollection<string> systemDimensionsValues;
            readonly IFabricMeterProvider fabricMeterProvider = new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object;
            readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

            public CreateMeter()
            {
                sut = new Int64MeterProvider(serviceContext);
                systemDimensionsNames = sut.Private().Field<IReadOnlyCollection<string>>().Value;
                systemDimensionsValues = sut.Protected().Field<IReadOnlyCollection<string>>().Value;
                sut.Field<IFabricMeterProvider>().Set(fabricMeterProvider);

                Mock.Get(fabricMeterProvider).Setup(x => x.CreateMeter(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<uint>(), It.IsAny<string[]>(), It.IsAny<uint>(), It.IsAny<string[]>())).Returns(fabricMeter);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter()
            {
                var combinedDimensionNames = systemDimensionsNames.ToArray();
                var fixedDimensionValues = systemDimensionsValues.ToArray();

                IMeter<long> meter = sut.CreateMeter(testNamespace, testMetric);

                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensionNames.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensionNames)), (uint)fixedDimensionValues.Length, It.Is<string[]>(arr => arr.SequenceEqual(fixedDimensionValues))), Times.Once);
                Assert.Same(fabricMeter, ((Int64Meter)meter).Field<IFabricMeter>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter1D()
            {
                var combinedDimensionNames = new List<string>(systemDimensionsNames) { testDimension1 }.ToArray();
                var fixedDimensionValues = systemDimensionsValues.ToArray();

                IMeter1D<long> meter1D = sut.CreateMeter(testNamespace, testMetric, testDimension1);

                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensionNames.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensionNames)), (uint)fixedDimensionValues.Length, It.Is<string[]>(arr => arr.SequenceEqual(fixedDimensionValues))), Times.Once);
                Assert.Same(fabricMeter, ((Int64Meter1D)meter1D).Field<IFabricMeter>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter2D()
            {
                var combinedDimensionNames = new List<string>(systemDimensionsNames) { testDimension1, testDimension2 }.ToArray();
                var fixedDimensionValues = systemDimensionsValues.ToArray();

                IMeter2D<long> meter2D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2);

                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensionNames.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensionNames)), (uint)fixedDimensionValues.Length, It.Is<string[]>(arr => arr.SequenceEqual(fixedDimensionValues))), Times.Once);
                Assert.Same(fabricMeter, ((Int64Meter2D)meter2D).Field<IFabricMeter>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter3D()
            {
                var combinedDimensionNames = new List<string>(systemDimensionsNames) { testDimension1, testDimension2, testDimension3 }.ToArray();
                var fixedDimensionValues = systemDimensionsValues.ToArray();

                IMeter3D<long> meter3D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2, testDimension3);

                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensionNames.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensionNames)), (uint)fixedDimensionValues.Length, It.Is<string[]>(arr => arr.SequenceEqual(fixedDimensionValues))), Times.Once);
                Assert.Same(fabricMeter, ((Int64Meter3D)meter3D).Field<IFabricMeter>().Value);
            }
        }
    }
}
