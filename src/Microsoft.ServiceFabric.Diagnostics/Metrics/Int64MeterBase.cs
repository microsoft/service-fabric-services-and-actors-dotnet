using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Interop;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class Int64MeterBase
    {
        readonly protected IList<string> systemDimensionValues;
        readonly protected IFabricMeter fabricMeter;
        Action<IFabricMeter, long, string[]> NativeRecord => (meter, value, dimensions) => meter.Record(value, dimensions, (uint)dimensions.Length);

        public Int64MeterBase(IFabricMeter fabricMeter, IList<string> systemDimensionValues)
        {
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
            this.systemDimensionValues = systemDimensionValues ?? new List<string>();
        }
    }
}
