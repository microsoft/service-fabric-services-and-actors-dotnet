// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class Int64Meter2D : Meter2D, IMeter2D<long>
    {
        internal Int64Meter2D(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        public new void Record(long value, string dimension1, string dimension2)
        {
            base.Record(value, dimension1, dimension2);
        }
    }
}
