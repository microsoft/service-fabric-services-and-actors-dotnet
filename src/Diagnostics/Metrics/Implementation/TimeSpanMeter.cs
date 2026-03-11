// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class TimeSpanMeter : Meter, IMeter<TimeSpan>
    {
        internal TimeSpanMeter(IFabricMeter fabricMeter, IReadOnlyCollection<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        void IMeter<TimeSpan>.Record(TimeSpan value)
        {
            Record(ConvertTimeSpanToLong(value));
        }
    }
}
