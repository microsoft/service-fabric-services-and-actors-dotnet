
using System;
using System.Collections.Generic;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class MeterBaseTest
    {
        readonly internal IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

        public class Constructor : MeterBaseTest
        {
            [Fact]
            public void SholdThrowArgumentNullExceptionWhenMeterNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => new MeterBase(null, new List<string>()));
            }

            [Fact]
            public void ShouldSetEmptySystemDimensionValuesWhenNullProvided()
            {
                var meter = new MeterBase(fabricMeter, null);

                var actualList = meter.Field<IList<string>>().Value;
                Assert.NotNull(actualList);
                Assert.Empty(actualList);
            }

            [Fact]
            public void ShouldSetSystemDimensionAndMeterValuesWhenProvided()
            {
                var systemDimensionValues = new List<string> { "value1", "value2" };
                var meter = new MeterBase(fabricMeter, systemDimensionValues);

                var actualList = meter.Field<IList<string>>().Value;
                Assert.NotNull(actualList);
                Assert.Equal(systemDimensionValues, actualList);


                var createdMeter = meter.Field<IFabricMeter>().Value;
                Assert.NotNull(createdMeter);
                Assert.Equal(fabricMeter, createdMeter);
            }
        }
    }
}
