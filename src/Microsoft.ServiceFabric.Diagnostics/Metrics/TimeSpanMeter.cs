// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    class TimeSpanMeter : Meter, IMeter<TimeSpan>
    {
        internal TimeSpanMeter(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        public void Record(TimeSpan value)
        {
            string[] allDimensionArray = systemDimensionValues.ToArray();
            fabricMeter.Record((long)Math.Round(value.TotalMilliseconds), (uint)allDimensionArray.Length, allDimensionArray);
        }
    }
}
