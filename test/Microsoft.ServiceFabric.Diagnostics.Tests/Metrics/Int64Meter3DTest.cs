using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class Int64Meter3DTest
    {
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();

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
            public void CallsFabricMeterRecord()
            {
                var systemDimenisions = new List<string> { "systemDimension1" };
                var meter = new Int64Meter3D(fabricMeter, systemDimenisions);
                meter.Record(42, "customDimension1", "customDimension2", "customDimension3");
                Mock.Get(fabricMeter).Verify(m => m.Record(42, It.Is<string[]>(arr => arr.Length == 4 && arr[0] == "systemDimension1" && arr[1] == "customDimension1" && arr[2] == "customDimension2" && arr[3] == "customDimension3"), 4), Times.Once);
            }
        }
    }
}
