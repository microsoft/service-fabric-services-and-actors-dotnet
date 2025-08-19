using System;
using System.Collections.Generic;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Interop;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64Meter3DTest
    {
        readonly internal IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

        public class Constructor : Int64Meter3DTest
        {
            [Fact]
            public void SholdThrowArgumentNullExceptionWhenMeterNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => new Int64Meter3D(null, new List<string>()));
            }

            [Fact]
            public void ShouldSetEmptySystemDimensionValuesWhenNoneProvided()
            {
                var meter = new Int64Meter3D(fabricMeter, null);

                var actualList = meter.Field<IList<string>>().Value;
                Assert.NotNull(actualList);
                Assert.Empty(actualList);
            }
        }
    }
}
