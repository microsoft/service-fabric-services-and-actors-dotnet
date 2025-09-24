// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public abstract class TimeSpanMeter2DTest
    {
        readonly IMeter2D<TimeSpan> sut;

        // Constructor parameters
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String());

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public TimeSpanMeter2DTest() => sut = new TimeSpanMeter2D(fabricMeter, systemDimensions);

        public class Class : TimeSpanMeter2DTest
        {
            [Fact]
            public void InvokesBaseWithGivenArguments()
            {
                var meter = (Meter)sut;
                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensions, meter.Field<string[]>().Value);
            }
        }

        public class Record : TimeSpanMeter2DTest
        {
            readonly TimeSpan value = fuzzy.TimeSpan();
            readonly string customDimension1 = fuzzy.String();
            readonly string customDimension2 = fuzzy.String();

            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2 }).ToArray();
                long expectedLongValue = (long)Math.Round(value.TotalMilliseconds);

                sut.Record(value, customDimension1, customDimension2);

                Mock.Get(fabricMeter).Verify(m => m.Record(expectedLongValue, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
        }
    }
}
