// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Interop;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class Int64Meter3D : IMeter3D<long>
    {
        readonly IList<string> dimensionValues;
        readonly IFabricMeter fabricMeter;

        public Int64Meter3D(IFabricMeter fabricMeter, IList<string> dimensionValues)
        {
            this.fabricMeter = fabricMeter;
            this.dimensionValues = dimensionValues;
        }

        public void Record(long value, string dimension1, string dimension2, string dimension3)
        {
            throw new NotImplementedException();
        }
    }
}
