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
        readonly Meter sut;

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String());
        readonly long value = fuzzy.Int64();

        protected string[] recordedArray;

        public MeterTest()
        {
            sut = new Mock<Meter>(fabricMeter, systemDimensions).Object;

            // capture strings emitted to IFabricMeter.Record for assertion in tests
            Mock.Get(fabricMeter)
                .Setup(m => m.Record(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback<long, uint, IntPtr>((value, count, stringPtrs) => recordedArray = Util.CaptureStringPointers(stringPtrs, count));
        }

        public class Constructor : MeterTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenMeterNameIsNull()
            {
                var exception = Xunit.Record.Exception(() => new Mock<Meter>(null, systemDimensions).Object);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenSystemDimensionsAreNull()
            {
                var exception = Xunit.Record.Exception(() => new Mock<Meter>(fabricMeter, null).Object);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
            }

            [Fact]
            public void SetsAllFieldsWhenSystemDimensionsAreEmpty()
            {
                Meter meter = new Mock<Meter>(fabricMeter, Array.Empty<string>()).Object;

                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(Array.Empty<string>(), meter.Field<IReadOnlyCollection<string>>().Value);
            }

            [Fact]
            public void SetsAllFields()
            {
                Assert.Same(fabricMeter, sut.Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensions, sut.Field<IReadOnlyCollection<string>>().Value);
            }
        }

        public class Record : MeterTest
        {
            readonly Method<Action<long>> sutMethod;

            public Record() => sutMethod = sut.Protected().Method<Action<long>>();

            [Fact]
            public void CallsRecordAction()
            {
                var expectedArray = systemDimensions.ToArray();

                sutMethod.Invoke(value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, recordedArray);
            }
        }

        public class RecordViaNative : MeterTest
        {
            readonly string customDimension1 = fuzzy.String();
            readonly string customDimension2 = fuzzy.String();
            readonly string customDimension3 = fuzzy.String();

            readonly Method<Action<long, int, string, string, string>> sutMethod;

            public RecordViaNative()
            {
                sutMethod = sut.Protected().Method<Action<long, int, string, string, string>>();
            }


            [Fact]
            public void ThrowsExceptionIfNumberOfCustomDimensionsNegative()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    sutMethod.Invoke(value, fuzzy.Int32().Maximum(-1), customDimension1, customDimension2, customDimension3));
            }

            [Fact]
            public void ThrowsExceptionIfNumberOfCustomDimensionsHigherThanSupported()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    sutMethod.Invoke(value, fuzzy.Int32().Minimum(4), customDimension1, customDimension2, customDimension3));
            }

            [Fact]
            public void CallsNativeMeterRecordWithZeroCustomDimensionsAndAllSystemDimensions()
            {
                var expectedArray = systemDimensions.ToArray();

                sutMethod.Invoke(value, 0, customDimension1, customDimension2, customDimension3);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, recordedArray);
            }

            [Fact]
            public void CallsNativeMeterRecordWithOnCustomDimensionAndAllSystemDimensions()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1 }).ToArray();

                sutMethod.Invoke(value, 1, customDimension1, customDimension2, customDimension3);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, recordedArray);
            }

            [Fact]
            public void CallsNativeMeterRecordWithTwoCustomDimensionsAndAllSystemDimensions()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2 }).ToArray();

                sutMethod.Invoke(value, 2, customDimension1, customDimension2, customDimension3);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, recordedArray);
            }

            [Fact]
            public void CallsNativeMeterRecordWithThreeCustomDimensionsAndAllSystemDimensions()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2, customDimension3 }).ToArray();

                sutMethod.Invoke(value, 3, customDimension1, customDimension2, customDimension3);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, recordedArray);
            }
        }
    }
}
