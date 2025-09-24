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

            readonly long value = fuzzy.Int64();
            readonly string customDimension1 = fuzzy.String();
            readonly string customDimension2 = fuzzy.String();
            readonly string customDimension3 = fuzzy.String();

            readonly Method<Action<long, int, string, string, string>> sutMethod;

            public Record()
            {
                sut = new MeterImplementation(fabricMeter, systemDimensions);
                sutMethod = sut.Method<Action<long, int, string, string, string>>();
            }

            [Fact]
            public void ThrowsExceptionIfNumberOfCustomDimenionsNegative()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    sutMethod.Invoke(value, fuzzy.Int32().Maximum(-1), customDimension1, customDimension2, customDimension3));
            }

            [Fact]
            public void ThrowsExceptionIfNumberOfCustomDimenionsHigherThanSupported()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    sutMethod.Invoke(value, fuzzy.Int32().Minimum(4), customDimension1, customDimension2, customDimension3));
            }

            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions0D()
            {
                var expectedArray = systemDimensions.ToArray();

                sutMethod.Invoke(value, 0, customDimension1, customDimension2, customDimension3);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions1D()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1 }).ToArray();

                sutMethod.Invoke(value, 1, customDimension1, customDimension2, customDimension3);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions2D()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2 }).ToArray();

                sutMethod.Invoke(value, 2, customDimension1, customDimension2, customDimension3);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions3D()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2, customDimension3 }).ToArray();

                sutMethod.Invoke(value, 3, customDimension1, customDimension2, customDimension3);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
        }

        class MeterImplementation : Meter
        {
            public MeterImplementation(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }
        }
    }
}
