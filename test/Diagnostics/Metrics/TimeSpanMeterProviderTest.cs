// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Runtime.InteropServices;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public class TimeSpanMeterProviderTest : FabricTelemetryDllFixture
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly ServiceContext serviceContext = fuzzy.ServiceContext();

        public class CreateMeter : TimeSpanMeterProviderTest
        {
            readonly string testNamespace = fuzzy.String();
            readonly string testMetric = fuzzy.String();
            readonly string testDimension1 = fuzzy.String();
            readonly string testDimension2 = fuzzy.String();
            readonly string testDimension3 = fuzzy.String();

            readonly TimeSpanMeterProvider sut;
            readonly IReadOnlyCollection<string> systemDimensionsNames;
            readonly IReadOnlyCollection<string> systemDimensionsValues;
            readonly IFabricMeterProvider2 fabricMeterProvider = new Mock<IFabricMeterProvider2>() { DefaultValue = DefaultValue.Mock }.Object;
            readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

            // Values captured from FABRIC_METER_DESCRIPTION during CreateMeter call
            string actualNamespace;
            string actualName;
            string[] actualDimensionNames;
            string[] actualFixedDimensionValues;

            public CreateMeter()
            {
                sut = new TimeSpanMeterProvider(serviceContext);
                systemDimensionsNames = sut.Private().Field<IReadOnlyCollection<string>>().Value;
                systemDimensionsValues = sut.Protected().Field<IReadOnlyCollection<string>>().Value;
                sut.Field<IFabricMeterProvider2>().Set(fabricMeterProvider);

                Mock.Get(fabricMeterProvider)
                    .Setup(x => x.CreateMeter2(It.IsAny<IntPtr>()))
                    .Callback((IntPtr ptr) =>
                    {
                        var desc = Marshal.PtrToStructure<FABRIC_METER_DESCRIPTION>(ptr);
                        actualNamespace = Marshal.PtrToStringUni(desc.Namespace);
                        actualName = Marshal.PtrToStringUni(desc.Name);
                        actualDimensionNames = PtrToStringArray(desc.DimensionNames, (int)desc.TotalDimensionsCount);
                        actualFixedDimensionValues = PtrToStringArray(desc.FixedDimensionValues, (int)desc.FixedDimensionCount);
                    })
                    .Returns(fabricMeter);
            }

            static string[] PtrToStringArray(IntPtr array, int count)
            {
                var result = new string[count];
                for (int i = 0; i < count; i++)
                    result[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(array, i * IntPtr.Size));
                return result;
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter()
            {
                IMeter<TimeSpan> meter = sut.CreateMeter(testNamespace, testMetric);

                Assert.Equal(testNamespace, actualNamespace);
                Assert.Equal(testMetric, actualName);
                Assert.Equal(systemDimensionsNames, actualDimensionNames);
                Assert.Equal(systemDimensionsValues, actualFixedDimensionValues);
                Assert.Same(fabricMeter, ((TimeSpanMeter)meter).Field<IFabricMeter>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter1D()
            {
                IMeter1D<TimeSpan> meter1D = sut.CreateMeter(testNamespace, testMetric, testDimension1);

                Assert.Equal(testNamespace, actualNamespace);
                Assert.Equal(testMetric, actualName);
                Assert.Equal([.. systemDimensionsNames, testDimension1], actualDimensionNames);
                Assert.Equal(systemDimensionsValues, actualFixedDimensionValues);
                Assert.Same(fabricMeter, ((TimeSpanMeter1D)meter1D).Field<IFabricMeter>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter2D()
            {
                IMeter2D<TimeSpan> meter2D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2);

                Assert.Equal(testNamespace, actualNamespace);
                Assert.Equal(testMetric, actualName);
                Assert.Equal([.. systemDimensionsNames, testDimension1, testDimension2], actualDimensionNames);
                Assert.Equal(systemDimensionsValues, actualFixedDimensionValues);
                Assert.Same(fabricMeter, ((TimeSpanMeter2D)meter2D).Field<IFabricMeter>().Value);
            }

            [Fact]
            public void CreatesMeterWithCorrectDimensionsAndMeter3D()
            {
                IMeter3D<TimeSpan> meter3D = sut.CreateMeter(testNamespace, testMetric, testDimension1, testDimension2, testDimension3);

                Assert.Equal(testNamespace, actualNamespace);
                Assert.Equal(testMetric, actualName);
                Assert.Equal([.. systemDimensionsNames, testDimension1, testDimension2, testDimension3], actualDimensionNames);
                Assert.Equal(systemDimensionsValues, actualFixedDimensionValues);
                Assert.Same(fabricMeter, ((TimeSpanMeter3D)meter3D).Field<IFabricMeter>().Value);
            }
        }
    }
}
