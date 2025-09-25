// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
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
            readonly long longValue;
            readonly string customDimension1 = fuzzy.String();
            readonly string customDimension2 = fuzzy.String();

            readonly Action<long, int, string, string, string> mockRecordAction = Mock.Of<Action<long, int, string, string, string>>();

            public Record()
            {
                sut.Private().Field<Action<long, int, string, string, string>>().Set(mockRecordAction);
                longValue = (long)Math.Round(value.TotalMilliseconds);
            }

            [Fact]
            public void InvokesBaseRecord()
            {
                sut.Record(value, customDimension1, customDimension2);

                Mock.Get(mockRecordAction).Verify(m => m.Invoke(longValue, 2, customDimension1, customDimension2, null), Times.Once);
            }
        }
    }
}
