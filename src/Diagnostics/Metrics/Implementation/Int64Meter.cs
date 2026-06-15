// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class Int64Meter : Meter, IMeter<long>
    {
        internal Int64Meter(IFabricMeter fabricMeter) : base(fabricMeter) { }

        void IMeter<long>.Record(long value)
        {
            base.Record(value);
        }
    }
}
