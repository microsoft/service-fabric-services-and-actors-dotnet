// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class Int64Meter : Meter, IMeter<long>
    {
        internal Int64Meter(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        public void Record(long value)
        {
            fabricMeter.Record(value, (uint)systemDimensionValues.Length, systemDimensionValues);
        }
    }
}
