// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class TimeSpanMeter3D : Meter3D, IMeter3D<TimeSpan>
    {
        internal TimeSpanMeter3D(IFabricMeter fabricMeter) : base(fabricMeter) { }

        void IMeter3D<TimeSpan>.Record(TimeSpan value, string dimension1Value, string dimension2Value, string dimension3Value)
        {
            Record(ConvertTimeSpanToLong(value), dimension1Value, dimension2Value, dimension3Value);
        }
    }
}
