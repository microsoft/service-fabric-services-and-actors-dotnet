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
    public abstract class Int64MeterTest
    {
        readonly IMeter<long> sut;

        // Constructor parameters
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String(), Count.Min(1));

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public Int64MeterTest() => sut = new Int64Meter(fabricMeter, systemDimensions);

        public class Class : Int64MeterTest
        {
            [Fact]
            public void InvokesBaseWithGivenArguments()
            {
                var meter = (Meter)sut;
                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Same(systemDimensions, meter.Field<IEnumerable<string>>().Value);
            }
        }

        public class Record : Int64MeterTest
        {
            readonly long value = fuzzy.Int64();

            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions()
            {
                // Arrange
                string[] expectedArray = systemDimensions.ToArray();

                // Act
                sut.Record(value);

                // Assert
                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }

            [Fact]
            public void CallsFabricMeterRecordWithNoSystemDimensions()
            {
                // Arrange
                string[] expectedArray = new string[0];

                // Act
                var meter = new Int64Meter(fabricMeter, new List<string>());
                meter.Record(value);

                // Assert
                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
        }
    }
}
