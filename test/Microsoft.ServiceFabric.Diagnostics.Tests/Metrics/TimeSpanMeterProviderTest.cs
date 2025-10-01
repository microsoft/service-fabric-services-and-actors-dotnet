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
    public class TimeSpanMeterProviderTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly ServiceContext serviceContext = fuzzy.ServiceContext();

        public TimeSpanMeterProviderTest()
        {
            typeof(MeterProvider<TimeSpan>).Field<Func<IFabricMeterProvider>>().Set(() => Mock.Of<IFabricMeterProvider>());
        }

        public class CreateMeter : TimeSpanMeterProviderTest
        {
            readonly string testNamespace = fuzzy.String();
            readonly string testMetric = fuzzy.String();
            readonly string testDimension1 = fuzzy.String();
            readonly string testDimension2 = fuzzy.String();
            readonly string testDimension3 = fuzzy.String();

            readonly TimeSpanMeterProvider sut;
            readonly IEnumerable<string> systemDimensionsNames;
            readonly IEnumerable<string> systemDimensionsValues;
            readonly IFabricMeterProvider fabricMeterProvider = new Mock<IFabricMeterProvider>() { DefaultValue = DefaultValue.Mock }.Object;
            readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

            public CreateMeter()
            {
                sut = new TimeSpanMeterProvider(serviceContext);
                systemDimensionsNames = sut.Private().Field<IEnumerable<string>>().Value;
                systemDimensionsValues = sut.Protected().Field<IEnumerable<string>>().Value;
                sut.Field<IFabricMeterProvider>().Set(fabricMeterProvider);

                Mock.Get(fabricMeterProvider).Setup(x => x.CreateMeter(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<uint>(), It.IsAny<string[]>())).Returns(fabricMeter);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter()
            {
                var combinedDimensions = systemDimensionsNames.ToArray();

                IMeter<TimeSpan> meter = sut.CreateMeter(testNamespace, testMetric);

                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensions.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensions))), Times.Once);
                Assert.Same(fabricMeter, ((TimeSpanMeter)meter).Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensionsValues, ((TimeSpanMeter)meter).Field<string[]>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter1D()
            {
                var combinedDimensions = new List<string>(systemDimensionsNames) { testDimension1 }.ToArray();

                IMeter1D<TimeSpan> meter1D = sut.CreateMeter(testNamespace, testMetric, testDimension1);

                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensions.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensions))), Times.Once);
                Assert.Same(fabricMeter, ((TimeSpanMeter1D)meter1D).Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensionsValues, ((TimeSpanMeter1D)meter1D).Field<string[]>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter2D()
            {
                var combinedDimensions = new List<string>(systemDimensionsNames) { testDimension1, testDimension2 }.ToArray();

                IMeter2D<TimeSpan> meter2D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2);

                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensions.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensions))), Times.Once);
                Assert.Same(fabricMeter, ((TimeSpanMeter2D)meter2D).Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensionsValues, ((TimeSpanMeter2D)meter2D).Field<string[]>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter3D()
            {
                var combinedDimensions = new List<string>(systemDimensionsNames) { testDimension1, testDimension2, testDimension3 }.ToArray();

                IMeter3D<TimeSpan> meter3D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2, testDimension3);

                Mock.Get(fabricMeterProvider).Verify(x => x.CreateMeter(testNamespace, testMetric, (uint)combinedDimensions.Length, It.Is<string[]>(arr => arr.SequenceEqual(combinedDimensions))), Times.Once);
                Assert.Same(fabricMeter, ((TimeSpanMeter3D)meter3D).Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensionsValues, ((TimeSpanMeter3D)meter3D).Field<string[]>().Value);
            }
        }
    }
}
