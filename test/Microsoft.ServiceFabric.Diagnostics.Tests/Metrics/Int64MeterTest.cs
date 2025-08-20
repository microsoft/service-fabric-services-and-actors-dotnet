using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64MeterTest
    {
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

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
            public void CallsFabricMeterRecord()
            {
                var systemDimenisions = new List<string> { "systemDimension1", "systemDimension2" };
                var meter = new Int64Meter(fabricMeter, systemDimenisions);
                meter.Record(42);
                Mock.Get(fabricMeter).Verify(m => m.Record(42, It.Is<string[]>(arr => arr.Length == 2 && arr[0] == "systemDimension1" && arr[1] == "systemDimension2"), 2), Times.Once);
            }
        }
    }
}
