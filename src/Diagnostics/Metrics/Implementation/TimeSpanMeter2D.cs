// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class TimeSpanMeter2D : Meter2D, IMeter2D<TimeSpan>
    {
        internal TimeSpanMeter2D(IFabricMeter fabricMeter) : base(fabricMeter) { }

        void IMeter2D<TimeSpan>.Record(TimeSpan value, string dimension1Value, string dimension2Value)
        {
            Record(ConvertTimeSpanToLong(value), dimension1Value, dimension2Value);
        }
    }
}
