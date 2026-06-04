// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class TimeSpanMeter : Meter, IMeter<TimeSpan>
    {
        internal TimeSpanMeter(IFabricMeter fabricMeter) : base(fabricMeter) { }

        void IMeter<TimeSpan>.Record(TimeSpan value)
        {
            Record(ConvertTimeSpanToLong(value));
        }
    }
}
