// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class Int64Meter2D : Meter2D, IMeter2D<long>
    {
        internal Int64Meter2D(IFabricMeter fabricMeter) : base(fabricMeter) { }

        void IMeter2D<long>.Record(long value, string dimension1Value, string dimension2Value)
        {
            base.Record(value, dimension1Value, dimension2Value);
        }
    }
}
