using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter3D : Meter
    {
        internal Meter3D(IFabricMeter fabricMeter, IReadOnlyCollection<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        protected void Record(long value, string customDimension1, string customDimension2, string customDimension3)
        {
            _ = customDimension1 ?? throw new ArgumentNullException(nameof(customDimension1));
            _ = customDimension2 ?? throw new ArgumentNullException(nameof(customDimension2));
            _ = customDimension3 ?? throw new ArgumentNullException(nameof(customDimension3));
            base.RecordViaNative(value, 3, customDimension1, customDimension2, customDimension3);
        }
    }
}
