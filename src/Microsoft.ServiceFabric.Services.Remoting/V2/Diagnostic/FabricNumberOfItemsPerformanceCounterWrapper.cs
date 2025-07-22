using System.Fabric.Common;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class FabricNumberOfItemsPerformanceCounterWrapper : AbstractFabricCounterWriterWrapper
    {
        internal FabricNumberOfItemsPerformanceCounterWrapper(FabricNumberOfItems64PerformanceCounterWriter performanceCounterWriter)
            : base(performanceCounterWriter)
        {
        }

        internal override void UpdateCounterValue(long delta)
        {
            (this.performanceCounterWriter as FabricNumberOfItems64PerformanceCounterWriter).UpdateCounterValue(delta);
        }
    }
}
