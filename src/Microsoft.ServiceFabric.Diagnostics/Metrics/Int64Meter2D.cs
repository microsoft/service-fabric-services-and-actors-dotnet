// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Interop;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class Int64Meter2D : IMeter2D<long>
    {
        readonly IList<string> systemDimensionValues;
        readonly IFabricMeter fabricMeter;

        public Int64Meter2D(IFabricMeter fabricMeter, IList<string> systemDimensionValues)
        {
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
            this.systemDimensionValues = systemDimensionValues ?? new List<string>();
        }

        public void Record(long value, string dimension1, string dimension2)
        {
            throw new NotImplementedException();
        }
    }
}
