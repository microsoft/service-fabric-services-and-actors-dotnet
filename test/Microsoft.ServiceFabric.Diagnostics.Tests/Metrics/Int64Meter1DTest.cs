using System;
using System.Collections.Generic;
using System.Linq;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64Meter1DTest
    {
        readonly IMeter1D<long> sut;

        // Constructor parameters
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String(), Count.Min(1));

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public Int64Meter1DTest() => sut = new Int64Meter1D(fabricMeter, systemDimensions);

        public class Class : Int64Meter1DTest
        {
            [Fact]
            public void InvokesBaseWithGivenArguments()
            {
                var meter = (Meter)sut;
                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Same(systemDimensions, meter.Field<IEnumerable<string>>().Value);
            }
        }

        public class Record : Int64Meter1DTest
        {
            readonly string customDimension1 = fuzzy.String();
            readonly long value = fuzzy.Int64();

            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions()
            {
                // Arrange
                string[] expectedArray = systemDimensions.Concat(new[] { customDimension1 }).ToArray();

                // Act
                sut.Record(value, customDimension1);

                // Assert
                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }

            [Fact]
            public void CallsFabricMeterRecordWithNoSystemDimensions()
            {
                // Arrange
                string[] expectedArray = new List<string> { customDimension1 }.ToArray();

                // Act
                var meter = new Int64Meter1D(fabricMeter, new List<string>());
                meter.Record(value, customDimension1);

                // Assert
                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
        }
    }
}
