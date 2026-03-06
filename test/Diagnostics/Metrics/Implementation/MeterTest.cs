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
    public abstract class MeterTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String());

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
            public void SetsAllFieldsWhenSystemDimensionsAreEmpty()
            {
                var expectedSystemDimensions = new List<string>();
                Meter meter = new MeterImplementation(fabricMeter, expectedSystemDimensions);
                var expectedRecordAction = meter.Protected().Method<Action<long, int, string, string, string>>();

                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(expectedSystemDimensions, meter.Field<string[]>().Value);
                Assert.Equal(expectedRecordAction, meter.Field<Action<long, int, string, string, string>>().Value);
            }

            [Fact]
            public void SetsAllFields()
            {
                Meter meter = new MeterImplementation(fabricMeter, systemDimensions);
                var expectedRecordAction = meter.Protected().Method<Action<long, int, string, string, string>>();

                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensions, meter.Field<string[]>().Value);
                Assert.Equal(expectedRecordAction, meter.Field<Action<long, int, string, string, string>>().Value);
            }
        }

        public class Record : MeterTest
        {
            readonly Meter sut;

            protected string[] recordedArray;

            readonly long value = fuzzy.Int64();
            readonly string customDimension1 = fuzzy.String();
            readonly string customDimension2 = fuzzy.String();
            readonly string customDimension3 = fuzzy.String();

            readonly Method<Action<long, int, string, string, string>> oldSutMethod;
            readonly Method<Action<long>> sutMethod;
            readonly Action<long, int, string, string, string> mockRecordAction = Mock.Of<Action<long, int, string, string, string>>();

            public Record()
            {
                sut = new MeterImplementation(fabricMeter, systemDimensions);
                oldSutMethod = sut.Public().Method<Action<long, int, string, string, string>>();
                sutMethod = sut.Protected().Method<Action<long>>();

                // capture strings emitted to IFabricMeter.Record for assertion in tests
                Mock.Get(fabricMeter)
                    .Setup(m => m.Record(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                    .Callback<long, uint, IntPtr>((value, count, stringPtrs) => recordedArray = Util.CaputreStringPointers(stringPtrs, count));
            }

            [Fact]
            public void OLDCallsRecordAction()
            {
                sut.Private().Field<Action<long, int, string, string, string>>().Set(mockRecordAction);

                oldSutMethod.Invoke(value, 3, customDimension1, customDimension2, customDimension3);

                Mock.Get(mockRecordAction).Verify(a => a.Invoke(value, 3, customDimension1, customDimension2, customDimension3), Times.Once);
            }

            [Theory]
            [InlineData(null, "DimensionValue", "DimensionValue")]
            [InlineData("DimensionValue", null, "DimensionValue")]
            [InlineData("DimensionValue", "DimensionValue", null)]
            [InlineData("DimensionValue", null, null)]
            [InlineData(null, "DimensionValue", null)]
            [InlineData(null, null, "DimensionValue")]
            [InlineData(null, null, null)]
            public void OLDThrowsExceptionIfCustomDimensionIsExpectedButNull(string customDimension1, string customDimension2, string customDimension3)
            {
                Assert.Throws<ArgumentException>(() => oldSutMethod.Invoke(value, 3, customDimension1, customDimension2, customDimension3));
            }

            // NEW TESTS

            [Fact]
            public void CallsRecordAction()
            {
                var expectedArray = systemDimensions.ToArray();

                sutMethod.Invoke(value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, recordedArray);
            }

            public class RecordViaNative : Record
            {
                readonly Method<Action<long, int, string, string, string>> sutNativeMethod;

                public RecordViaNative()
                {
                    sutNativeMethod = sut.Protected().Method<Action<long, int, string, string, string>>();
                }


                [Fact]
                public void ThrowsExceptionIfNumberOfCustomDimensionsNegative()
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() =>
                        sutNativeMethod.Invoke(value, fuzzy.Int32().Maximum(-1), customDimension1, customDimension2, customDimension3));
                }

                [Fact]
                public void ThrowsExceptionIfNumberOfCustomDimensionsHigherThanSupported()
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() =>
                        sutNativeMethod.Invoke(value, fuzzy.Int32().Minimum(4), customDimension1, customDimension2, customDimension3));
                }

                [Fact]
                public void CallsNativeMeterRecordWithZeroCustomDimensionsAndAllSystemDimensions()
                {
                    var expectedArray = systemDimensions.ToArray();

                    sutNativeMethod.Invoke(value, 0, customDimension1, customDimension2, customDimension3);

                    Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                    Assert.Equal(expectedArray, recordedArray);
                }

                [Fact]
                public void CallsNativeMeterRecordWithOnCustomDimensionAndAllSystemDimensions()
                {
                    var expectedArray = systemDimensions.Concat(new[] { customDimension1 }).ToArray();

                    sutNativeMethod.Invoke(value, 1, customDimension1, customDimension2, customDimension3);

                    Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                    Assert.Equal(expectedArray, recordedArray);
                }

                [Fact]
                public void CallsNativeMeterRecordWithTwoCustomDimensionsAndAllSystemDimensions()
                {
                    var expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2 }).ToArray();

                    sutNativeMethod.Invoke(value, 2, customDimension1, customDimension2, customDimension3);

                    Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                    Assert.Equal(expectedArray, recordedArray);
                }

                [Fact]
                public void CallsNativeMeterRecordWithThreeCustomDimensionsAndAllSystemDimensions()
                {
                    var expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2, customDimension3 }).ToArray();

                    sutNativeMethod.Invoke(value, 3, customDimension1, customDimension2, customDimension3);

                    Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                    Assert.Equal(expectedArray, recordedArray);
                }


            }
        }

        private class MeterImplementation : Meter
        {
            public MeterImplementation(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }
        }
    }
}
