using System.Fabric.Common;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class FabricAverageCountPerformanceCounterWrapper : AbstractFabricCounterWriterWrapper
    {
        internal FabricAverageCountPerformanceCounterWrapper(FabricAverageCount64PerformanceCounterWriter performanceCounterWriter)
            : base(performanceCounterWriter)
        {
        }

        internal override void UpdateCounterValue(long delta)
        {
            (this.performanceCounterWriter as FabricAverageCount64PerformanceCounterWriter).UpdateCounterValue(delta);
        }
    }
}
