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
    public abstract class MeterTest
    {
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String());

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public class Constructor : MeterTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenMeterNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => new MeterImplementation(null, systemDimensions));
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenSystemDimensionsAreNull()
            {
                Assert.Throws<ArgumentNullException>(() => new MeterImplementation(fabricMeter, null));
            }

            [Fact]
            public void SetsSystemDimensionAndMeterValuesWhenSystemDimensionsAreEmpty()
            {
                var expectedSystemDimensions = new List<string>();
                Meter meter = new MeterImplementation(fabricMeter, expectedSystemDimensions);

                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(expectedSystemDimensions, meter.Field<string[]>().Value);
            }

            [Fact]
            public void SetsSystemDimensionAndMeter()
            {
                Meter meter = new MeterImplementation(fabricMeter, systemDimensions);

                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensions, meter.Field<string[]>().Value);
            }
        }

        public class Record : MeterTest
        {
            readonly Meter sut;

            readonly List<string> customDimensions = fuzzy.List(() => fuzzy.String());
            readonly long value = fuzzy.Int64();

            public Record() => sut = new MeterImplementation(fabricMeter, systemDimensions);

            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions()
            {
                var expectedArray = systemDimensions.Concat(customDimensions).ToArray();

                sut.Method<Action<long, string[]>>().Invoke(value, customDimensions.ToArray());

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
        }

        class MeterImplementation : Meter
        {
            public MeterImplementation(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }
        }
    }
}
