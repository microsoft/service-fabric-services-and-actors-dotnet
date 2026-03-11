// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class TimeSpanMeter1D : Meter1D, IMeter1D<TimeSpan>
    {
        internal TimeSpanMeter1D(IFabricMeter fabricMeter, IReadOnlyCollection<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        void IMeter1D<TimeSpan>.Record(TimeSpan value, string dimension1Value)
        {
            Record(ConvertTimeSpanToLong(value), dimension1Value);
        }
    }
}
