using System;
using System.Collections.Generic;
using System.Linq;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Tests.Metrics.Implementation
{
    public abstract class Meter1DTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String());

        public class Constructor : Meter1DTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenMeterIsNull()
            {
                var exception = Xunit.Record.Exception(() => new Mock<Meter1D>(null, systemDimensions).Object);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenSystemDimensionsAreNull()
            {
                var exception = Xunit.Record.Exception(() => new Mock<Meter1D>(fabricMeter, null).Object);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
            }
        }

        public class Record : Meter1DTest
        {
            readonly Meter sut;

            readonly Method<Action<long, string>> sutMethod;

            protected string[] recordedArray;
            readonly long value = fuzzy.Int64();
            readonly string customDimension1 = fuzzy.String();

            public Record()
            {
                sut = new Mock<Meter1D>(fabricMeter, systemDimensions).Object;
                sutMethod = sut.Protected().Method<Action<long, string>>();

                // capture strings emitted to IFabricMeter.Record for assertion in tests
                Mock.Get(fabricMeter)
                    .Setup(m => m.Record(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                    .Callback<long, uint, IntPtr>((value, count, stringPtrs) => recordedArray = Util.CaputreStringPointers(stringPtrs, count));
            }

            [Fact]
            public void CallsRecordAction()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1 }).ToArray();

                sutMethod.Invoke(value, customDimension1);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.IsAny<IntPtr>()), Times.Once);
                Assert.Equal(expectedArray, recordedArray);
            }

            [Fact]
            public void ThrowsExceptionIfCustomDimensionIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => sutMethod.Invoke(value, null));
            }
        }
    }
}
