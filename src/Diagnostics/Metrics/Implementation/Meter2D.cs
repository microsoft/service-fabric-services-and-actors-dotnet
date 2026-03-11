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

        protected void Record(long value, string dimension1Value, string dimension2Value)
        {
            _ = dimension1Value ?? throw new ArgumentNullException(nameof(dimension1Value));
            _ = dimension2Value ?? throw new ArgumentNullException(nameof(dimension2Value));
            base.RecordViaNative(value, 2, dimension1Value, dimension2Value, null);
        }
    }
}
