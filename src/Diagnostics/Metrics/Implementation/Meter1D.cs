using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter1D : Meter
    {
        internal Meter1D(IFabricMeter fabricMeter, IReadOnlyCollection<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        protected void Record(long value, string customDimension1)
        {
            _ = customDimension1 ?? throw new ArgumentNullException(nameof(customDimension1));
            base.RecordViaNative(value, 1, customDimension1, null, null);
        }
    }
}
