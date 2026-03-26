// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Reflection;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public abstract class Meter1DTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

        public class Constructor : Meter1DTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenMeterIsNull()
            {
                var exception = Assert.Throws<TargetInvocationException>(() => new Mock<Meter1D>(null).Object);
                _ = Assert.IsType<ArgumentNullException>(exception.InnerException);
            }
        }

        public class Record : Meter1DTest
        {
            readonly Meter sut;

            readonly Method<Action<long, string>> sutMethod;

            protected string[] actualDimensions;
            readonly long value = fuzzy.Int64();
            readonly string dimension1Value = fuzzy.String();

            public Record()
            {
                sut = new Mock<Meter1D>(fabricMeter).Object;
                sutMethod = sut.Protected().Method<Action<long, string>>();

                // capture strings emitted to IFabricMeter.Record for assertion in tests
                Mock.Get(fabricMeter)
                    .Setup(m => m.Record(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                    .Callback<long, uint, IntPtr>((value, count, stringPtrs) => actualDimensions = Util.CaptureStringPointers(stringPtrs, count));
            }

            [Fact]
            public void CallsFabricMeterWithCustomDimensions()
            {
                string[] expectedArray = [dimension1Value];

                sutMethod.Invoke(value, dimension1Value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, actualDimensions);
            }

            [Fact]
            public void ThrowsExceptionIfCustomDimensionIsNull() => Assert.Throws<ArgumentNullException>(() => sutMethod.Invoke(value, null));
        }
    }
}
