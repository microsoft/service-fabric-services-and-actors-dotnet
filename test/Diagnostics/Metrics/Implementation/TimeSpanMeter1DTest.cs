// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Tests.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public abstract class TimeSpanMeter1DTest
    {
        readonly IMeter1D<TimeSpan> sut;

        // Constructor parameters
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String());

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public TimeSpanMeter1DTest() => sut = new TimeSpanMeter1D(fabricMeter, systemDimensions);

        public class Class : TimeSpanMeter1DTest
        {
            [Fact]
            public void InvokesBaseWithGivenArguments()
            {
                var meter = (Meter)sut;
                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensions, meter.Field<string[]>().Value);
            }
        }

        public class Record : TimeSpanMeter1DTest
        {
            readonly TimeSpan value = fuzzy.TimeSpan();
            readonly long longValue;
            readonly string customDimension1 = fuzzy.String();

            protected string[] recordedArray;

            public Record()
            {
                // capture strings emitted to IFabricMeter.Record for assertion in tests
                Mock.Get(fabricMeter)
                    .Setup(m => m.Record(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                    .Callback<long, uint, IntPtr>((value, count, stringPtrs) => recordedArray = Util.CaputreStringPointers(stringPtrs, count));

                longValue = (long)Math.Round(value.TotalMilliseconds);
            }

            [Fact]
            public void InvokesBaseRecord()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1 }).ToArray();

                sut.Record(value, customDimension1);

                Mock.Get(fabricMeter).Verify(m => m.Record(longValue, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, recordedArray);
            }
        }
    }
}
