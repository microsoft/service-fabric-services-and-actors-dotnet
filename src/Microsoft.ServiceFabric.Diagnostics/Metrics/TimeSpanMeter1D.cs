// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    class TimeSpanMeter1D : Meter, IMeter1D<TimeSpan>
    {
        internal TimeSpanMeter1D(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        public void Record(TimeSpan value, string dimension1)
        {
            var systemDimensionsList = (List<string>)systemDimensionValues;
            string[] allDimensionArray = new string[systemDimensionsList.Count + 1];

            systemDimensionsList.CopyTo(allDimensionArray, 0);
            allDimensionArray[allDimensionArray.Length - 1] = dimension1;

            fabricMeter.Record((long)Math.Round(value.TotalMilliseconds), (uint)allDimensionArray.Length, allDimensionArray);
        }
    }
}
