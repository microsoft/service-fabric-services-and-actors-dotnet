using System;
using System.Collections.Generic;
using System.Linq;
using Fuzzy;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64Meter3DTest
    {
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public class Class : Int64Meter3DTest
        {
            [Fact]
            public void InheritsFromInt64MeterBase()
            {
                var meter = new Int64Meter3D(fabricMeter, null);
                Assert.IsAssignableFrom<MeterBase>(meter);
            }
        }

        public class Record : Int64Meter3DTest
        {
            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions()
            {
                // Arrange
                List<string> systemDimensions = fuzzy.List(() => fuzzy.String(), Count.Between(1, 10));
                string customDimension1 = fuzzy.String();
                string customDimension2 = fuzzy.String();
                string customDimension3 = fuzzy.String();
                long value = fuzzy.Int64();
                string[] expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2, customDimension3 }).ToArray();

                // Act
                var meter = new Int64Meter3D(fabricMeter, systemDimensions);
                meter.Record(value, customDimension1, customDimension2, customDimension3);

                // Assert
                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }

            [Fact]
            public void CallsFabricMeterRecordWithNoSystemDimensions()
            {
                // Arrange
                List<string> systemDimensions = new List<string>();
                string customDimension1 = fuzzy.String();
                string customDimension2 = fuzzy.String();
                string customDimension3 = fuzzy.String();
                long value = fuzzy.Int64();
                string[] expectedArray = systemDimensions.Concat(new[] { customDimension1, customDimension2, customDimension3 }).ToArray();

                // Act
                var meter = new Int64Meter3D(fabricMeter, systemDimensions);
                meter.Record(value, customDimension1, customDimension2, customDimension3);

                // Assert
                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
        }
    }
}
