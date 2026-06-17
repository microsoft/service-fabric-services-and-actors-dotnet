// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class Int64Meter1D : Meter1D, IMeter1D<long>
    {
        internal Int64Meter1D(IFabricMeter fabricMeter) : base(fabricMeter) { }

        void IMeter1D<long>.Record(long value, string dimension1Value)
        {
            base.Record(value, dimension1Value);
        }
    }
}
