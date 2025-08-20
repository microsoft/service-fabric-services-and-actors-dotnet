// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class Int64Meter2D : MeterBase, IMeter2D<long>
    {
        public Int64Meter2D(IFabricMeter fabricMeter, IList<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues)
        {
        }

        public void Record(long value, string dimension1, string dimension2)
        {
            var allDimensionsList = new List<string>(systemDimensionValues)
            {
                dimension1,
                dimension2
            };
            string[] allDimensionArray = allDimensionsList.ToArray();
            fabricMeter.Record(value, allDimensionArray, (uint)allDimensionArray.Length);
        }
    }
}
