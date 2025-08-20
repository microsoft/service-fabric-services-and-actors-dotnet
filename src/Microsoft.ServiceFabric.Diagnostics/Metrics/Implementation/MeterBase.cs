using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    class MeterBase
    {
        protected readonly IList<string> systemDimensionValues;
        protected readonly IFabricMeter fabricMeter;

        internal MeterBase(IFabricMeter fabricMeter, IList<string> systemDimensionValues)
        {
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
            this.systemDimensionValues = systemDimensionValues ?? new List<string>();
        }
    }
}
