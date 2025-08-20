using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64Meter1DTest
    {
        readonly internal IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

        public class Class : Int64Meter1DTest
        {
            [Fact]
            public void InheritsFromInt64MeterBase()
            {
                var meter = new Int64Meter1D(fabricMeter, null);
                Assert.IsAssignableFrom<MeterBase>(meter);
            }
        }

        public class Record : Int64Meter1DTest
        {
            [Fact]
            public void CallsFabricMeterRecord()
            {
                var systemDimenisions = new List<string> { "systemDimension1", "systemDimension2" };
                var meter = new Int64Meter1D(fabricMeter, systemDimenisions);
                meter.Record(42, "customDimension1");
                Mock.Get(fabricMeter).Verify(m => m.Record(42, It.Is<string[]>(arr => arr.Length == 3 && arr[0] == "systemDimension1" && arr[1] == "systemDimension2" && arr[2] == "customDimension1"), 3), Times.Once);
            }
        }
    }
}
