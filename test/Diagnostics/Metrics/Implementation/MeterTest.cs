// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Interop;
using System.Reflection;
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
        readonly long value = fuzzy.Int64();

        protected string[] actualDimensions;

        public MeterTest()
        {
            sut = new Mock<Meter>(fabricMeter).Object;

            // capture strings emitted to IFabricMeter.Record for assertion in tests
            Mock.Get(fabricMeter)
                .Setup(m => m.Record(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback<long, uint, IntPtr>((value, count, stringPtrs) => actualDimensions = Util.CaptureStringPointers(stringPtrs, count));
        }

        public class Constructor : MeterTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenFabricMeterIsNull()
            {
                var exception = Assert.Throws<TargetInvocationException>(() => new Mock<Meter>(null).Object);
                _ = Assert.IsType<ArgumentNullException>(exception.InnerException);
            }

            [Fact]
            public void SetsFabricMeterField()
            {
                Assert.Same(fabricMeter, sut.Field<IFabricMeter>().Value);
            }
        }

        public class Record : MeterTest
        {
            readonly Method<Action<long>> sutMethod;

            public Record() => sutMethod = sut.Protected().Method<Action<long>>();

            [Fact]
            public void CallsFabricMeterWithNoDimensions()
            {
                string[] expectedArray = [];

                sutMethod.Invoke(value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, actualDimensions);
            }
        }

        public class DisposeTest : MeterTest, IDisposable
        {
            readonly Func<object, int> finalReleaseComObject = Mock.Of<Func<object, int>>();

            // Set the finalReleaseComObject delegate to a mock function, so we can verify that it was called in Dispose.
            public DisposeTest() =>
                typeof(Meter).Field<Func<object, int>>().Set(finalReleaseComObject);

            // Restore the original finalReleaseComObject delegate after the test, to avoid affecting other tests.
            public void Dispose() => typeof(Meter).Field<Func<object, int>>().Set(Utility.FinalReleaseComObject);

            [Fact]
            public void ReleasesNativeFabricMeter()
            {
                sut.Dispose();

                Mock.Get(finalReleaseComObject).Verify(f => f(fabricMeter), Times.Once);
                Assert.Null(sut.Private().Field<IFabricMeter>().Value);
            }

            [Fact]
            public void SubsequentDisposesDoNotReleaseNativeFabricMeterProvider()
            {
                sut.Dispose();
                sut.Dispose();

                Mock.Get(finalReleaseComObject).Verify(f => f(fabricMeter), Times.Once);
            }
        }

        public class RecordViaNative : DisposeTest
        {
            readonly string dimension1Value = fuzzy.String();
            readonly string dimension2Value = fuzzy.String();
            readonly string dimension3Value = fuzzy.String();

            readonly Method<Action<long, int, string, string, string>> sutMethod;

            public RecordViaNative() => sutMethod = sut.Protected().Method<Action<long, int, string, string, string>>();


            [Fact]
            public void ThrowsExceptionIfNumberOfCustomDimensionsNegative() =>
                Assert.Throws<ArgumentOutOfRangeException>(() => sutMethod.Invoke(value, fuzzy.Int32().Maximum(-1), dimension1Value, dimension2Value, dimension3Value));

            [Fact]
            public void ThrowsExceptionIfNumberOfCustomDimensionsHigherThanSupported() =>
                Assert.Throws<ArgumentOutOfRangeException>(() => sutMethod.Invoke(value, fuzzy.Int32().Minimum(4), dimension1Value, dimension2Value, dimension3Value));

            [Fact]
            public void CallsNativeMeterRecordWithZeroCustomDimensions()
            {
                string[] expectedArray = [];

                sutMethod.Invoke(value, 0, dimension1Value, dimension2Value, dimension3Value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, actualDimensions);
            }

            [Fact]
            public void CallsNativeMeterRecordWithOneCustomDimension()
            {
                string[] expectedArray = [dimension1Value];

                sutMethod.Invoke(value, 1, dimension1Value, dimension2Value, dimension3Value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, actualDimensions);
            }

            [Fact]
            public void CallsNativeMeterRecordWithTwoCustomDimensions()
            {
                string[] expectedArray = [dimension1Value, dimension2Value];

                sutMethod.Invoke(value, 2, dimension1Value, dimension2Value, dimension3Value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, actualDimensions);
            }

            [Fact]
            public void CallsNativeMeterRecordWithThreeCustomDimensions()
            {
                string[] expectedArray = [dimension1Value, dimension2Value, dimension3Value];

                sutMethod.Invoke(value, 3, dimension1Value, dimension2Value, dimension3Value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, actualDimensions);
            }


            [Fact]
            public void ThrowsIfDisposed()
            {
                sut.Dispose();

                _ = Assert.Throws<ObjectDisposedException>(() => sutMethod.Invoke(value, 3, dimension1Value, dimension2Value, dimension3Value));
            }
        }
    }
}
