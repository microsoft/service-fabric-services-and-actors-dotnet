using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class MeterBase
    {
        readonly protected IList<string> systemDimensionValues;
        readonly protected IFabricMeter fabricMeter;

        public MeterBase(IFabricMeter fabricMeter, IList<string> systemDimensionValues)
        {
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
            this.systemDimensionValues = systemDimensionValues ?? new List<string>();
        }
    }
}
