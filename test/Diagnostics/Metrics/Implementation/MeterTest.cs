// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            public void SetsAllFieldsWhenSystemDimensionsAreEmpty()
            {
                var expectedSystemDimensions = new List<string>();
                Meter meter = new MeterImplementation(fabricMeter, expectedSystemDimensions);
                var expectedRecordAction = meter.Private().Method<Action<long, int, string, string, string>>();

                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(expectedSystemDimensions, meter.Field<string[]>().Value);
                Assert.Equal(expectedRecordAction, meter.Field<Action<long, int, string, string, string>>().Value);
            }

            [Fact]
            public void SetsAllFields()
            {
                Meter meter = new MeterImplementation(fabricMeter, systemDimensions);
                var expectedRecordAction = meter.Private().Method<Action<long, int, string, string, string>>();

                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensions, meter.Field<string[]>().Value);
                Assert.Equal(expectedRecordAction, meter.Field<Action<long, int, string, string, string>>().Value);
            }
        }

        public class Record : MeterTest
        {
            readonly Meter sut;

            readonly long value = fuzzy.Int64();
            readonly string customDimension1 = fuzzy.String();
            readonly string customDimension2 = fuzzy.String();
            readonly string customDimension3 = fuzzy.String();
            private string[] recordedArray;

            readonly Method<Action<long, int, string, string, string>> sutMethod;
            readonly Action<long, int, string, string, string> mockRecordAction = Mock.Of<Action<long, int, string, string, string>>();

            public Record()
            {
                sut = new MeterImplementation(fabricMeter, systemDimensions);
                sutMethod = sut.Protected().Method<Action<long, int, string, string, string>>();

                // capture strings emitted to IFabricMeter.Record for assertion in tests
                Mock.Get(fabricMeter)
                    .Setup(m => m.Record(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                    .Callback<long, uint, IntPtr>((value, count, stringPtrs) => recordedArray = CaputreStringPointers(stringPtrs, count));
            }

            [Fact]
            public void CallsRecordAction()
            {
                sut.Private().Field<Action<long, int, string, string, string>>().Set(mockRecordAction);

                sutMethod.Invoke(value, 3, customDimension1, customDimension2, customDimension3);

                Mock.Get(mockRecordAction).Verify(a => a.Invoke(value, 3, customDimension1, customDimension2, customDimension3), Times.Once);
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

            [Theory]
            [InlineData(null, "DimensionValue", "DimensionValue")]
            [InlineData("DimensionValue", null, "DimensionValue")]
            [InlineData("DimensionValue", "DimensionValue", null)]
            [InlineData("DimensionValue", null, null)]
            [InlineData(null, "DimensionValue", null)]
            [InlineData(null, null, "DimensionValue")]
            [InlineData(null, null, null)]
            public void ThrowsExceptionIfCustomDimensionIsExpectedButNull(string customDimension1, string customDimension2, string customDimension3)
            {
                Assert.Throws<ArgumentException>(() => sutMethod.Invoke(value, 3, customDimension1, customDimension2, customDimension3));
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

            unsafe private static string[] CaputreStringPointers(IntPtr arrayPtr, uint arrayLength)
            {
                IntPtr* stringsPtr = (IntPtr*)arrayPtr;
                string[] capuredStrings = new string[arrayLength];

                for (int i = 0; i < arrayLength; i++)
                {
                    capuredStrings[i] = Marshal.PtrToStringUni(stringsPtr[i]);
                }

                return capuredStrings;
            }
        }

        class MeterImplementation : Meter
        {
            public MeterImplementation(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }
        }
    }
}
