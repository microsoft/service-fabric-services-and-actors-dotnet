using System;
using System.Collections.Generic;
using System.Linq;
using Fuzzy;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64MeterTest
    {
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public class Class : Int64MeterTest
        {
            [Fact]
            public void InheritsFromInt64MeterBase()
            {
                var meter = new Int64Meter(fabricMeter, null);
                Assert.IsAssignableFrom<MeterBase>(meter);
            }
        }

        public class Record : Int64MeterTest
        {
            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions()
            {
                // Arrange
                List<string> systemDimensions = fuzzy.List(() => fuzzy.String(), Count.Between(1, 10));
                long value = fuzzy.Int64();
                string[] expectedArray = systemDimensions.ToArray();

                // Act
                var meter = new Int64Meter(fabricMeter, systemDimensions);
                meter.Record(value);

                // Assert
                Mock.Get(fabricMeter).Verify(m => m.Record(value, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray)), (uint)expectedArray.Length), Times.Once);
            }

            [Fact]
            public void CallsFabricMeterRecordWithNoSystemDimensions()
            {
                // Arrange
                List<string> systemDimensions = new List<string>();
                long value = fuzzy.Int64();
                string[] expectedArray = systemDimensions.ToArray();

                // Act
                var meter = new Int64Meter(fabricMeter, systemDimensions);
                meter.Record(value);

                // Assert
                Mock.Get(fabricMeter).Verify(m => m.Record(value, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray)), (uint)expectedArray.Length), Times.Once);
            }
        }
    }
}
