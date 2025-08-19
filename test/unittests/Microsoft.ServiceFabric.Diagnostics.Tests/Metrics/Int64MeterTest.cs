using System;
using System.Collections.Generic;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Interop;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64MeterTest
    {
        readonly internal IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

        public class Constructor : Int64MeterTest
        {
            [Fact]
            public void SholdThrowArgumentNullExceptionWhenMeterNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => new Int64Meter(null, new List<string>()));
            }

            [Fact]
            public void ShouldSetEmptySystemDimensionValuesWhenNoneProvided()
            {
                var meter = new Int64Meter(fabricMeter, null);

                var actualList = meter.Field<IList<string>>().Value;
                Assert.NotNull(actualList);
                Assert.Empty(actualList);
            }
        }
    }
}
