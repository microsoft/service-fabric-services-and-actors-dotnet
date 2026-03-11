// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public abstract class Meter2DTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String());

        public class Constructor : Meter2DTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenMeterIsNull()
            {
                var exception = Assert.Throws<TargetInvocationException>(() => new Mock<Meter2D>(null, systemDimensions).Object);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenSystemDimensionsAreNull()
            {
                var exception = Assert.Throws<TargetInvocationException>(() => new Mock<Meter2D>(fabricMeter, null).Object);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
            }
        }

        public class Record : Meter2DTest
        {
            readonly Meter sut;

            readonly Method<Action<long, string, string>> sutMethod;

            protected string[] actualDimensions;
            readonly long value = fuzzy.Int64();
            readonly string dimension1Value = fuzzy.String();
            readonly string dimension2Value = fuzzy.String();

            public Record()
            {
                sut = new Mock<Meter2D>(fabricMeter, systemDimensions).Object;
                sutMethod = sut.Protected().Method<Action<long, string, string>>();

                // capture strings emitted to IFabricMeter.Record for assertion in tests
                Mock.Get(fabricMeter)
                    .Setup(m => m.Record(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                    .Callback<long, uint, IntPtr>((value, count, stringPtrs) => actualDimensions = Util.CaptureStringPointers(stringPtrs, count));
            }

            [Fact]
            public void CallsFabricMeterWithCombinedDimensions()
            {
                string[] expectedArray = systemDimensions.Concat(new[] { dimension1Value, dimension2Value }).ToArray();

                sutMethod.Invoke(value, dimension1Value, dimension2Value);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, actualDimensions);
            }

            [Theory]
            [InlineData(null, "dimension2Value")]
            [InlineData("dimension1Value", null)]
            [InlineData(null, null)]
            public void ThrowsExceptionIfCustomDimensionIsNull(string dimension1Value, string dimension2Value)
            {
                Assert.Throws<ArgumentNullException>(() => sutMethod.Invoke(value, dimension1Value, dimension2Value));
            }
        }
    }
}
