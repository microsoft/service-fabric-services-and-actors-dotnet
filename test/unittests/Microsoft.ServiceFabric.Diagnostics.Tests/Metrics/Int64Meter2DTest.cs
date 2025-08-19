using System;
using System.Collections.Generic;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Interop;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64Meter2DTest
    {
        readonly internal IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

        public class Constructor : Int64Meter2DTest
        {
            [Fact]
            public void SholdThrowArgumentNullExceptionWhenMeterNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => new Int64Meter2D(null, new List<string>()));
            }

            [Fact]
            public void ShouldSetEmptySystemDimensionValuesWhenNoneProvided()
            {
                var meter = new Int64Meter2D(fabricMeter, null);

                var actualList = meter.Field<IList<string>>().Value;
                Assert.NotNull(actualList);
                Assert.Empty(actualList);
            }
        }
    }
}
