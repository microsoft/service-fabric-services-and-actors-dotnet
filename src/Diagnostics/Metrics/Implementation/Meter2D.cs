// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter2D : Meter
    {
        internal Meter2D(IFabricMeter fabricMeter, IReadOnlyCollection<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        protected void Record(long value, string customDimension1, string customDimension2)
        {
            _ = customDimension1 ?? throw new ArgumentNullException(nameof(customDimension1));
            _ = customDimension2 ?? throw new ArgumentNullException(nameof(customDimension2));
            base.RecordViaNative(value, 2, customDimension1, customDimension2, null);
        }
    }
}
