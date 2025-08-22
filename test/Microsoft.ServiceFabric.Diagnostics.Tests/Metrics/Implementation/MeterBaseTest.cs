
using System;
using System.Collections.Generic;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public abstract class MeterBaseTest
    {
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public class Constructor : MeterBaseTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenMeterNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => new Meter(null, new List<string>()));
            }

            [Fact]
            public void SetsEmptySystemDimensionValuesWhenNullProvided()
            {
                var meter = new Meter(fabricMeter, null);

                var actualList = meter.Field<IEnumerable<string>>().Value;
                Assert.NotNull(actualList);
                Assert.Empty(actualList);
            }

            [Fact]
            public void SetsSystemDimensionAndMeterValuesWhenProvided()
            {
                var expectedValues = fuzzy.List(() => fuzzy.String());
                var meter = new Meter(fabricMeter, expectedValues);

                var actualList = meter.Field<IEnumerable<string>>().Value;
                Assert.NotNull(actualList);
                Assert.Equal(expectedValues, actualList);


                var createdMeter = meter.Field<IFabricMeter>().Value;
                Assert.NotNull(createdMeter);
                Assert.Equal(fabricMeter, createdMeter);
            }
        }
    }
}
