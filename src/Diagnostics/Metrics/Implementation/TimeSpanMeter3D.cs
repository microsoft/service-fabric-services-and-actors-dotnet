// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class TimeSpanMeter3D : Meter, IMeter3D<TimeSpan>
    {
        internal TimeSpanMeter3D(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        public void Record(TimeSpan value, string dimension1, string dimension2, string dimension3)
        {
            base.Record(ConvertTimeSpanToLong(value), 3, dimension1, dimension2, dimension3);
        }
    }
}
