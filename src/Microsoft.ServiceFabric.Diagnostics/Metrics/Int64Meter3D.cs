// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    sealed class Int64Meter3D : MeterBase, IMeter3D<long>
    {
        internal Int64Meter3D(IFabricMeter fabricMeter, IList<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        public void Record(long value, string dimension1, string dimension2, string dimension3)
        {
            var allDimensionsList = new List<string>(systemDimensionValues)
            {
                dimension1,
                dimension2,
                dimension3
            };
            string[] allDimensionArray = allDimensionsList.ToArray();
            fabricMeter.Record(value, allDimensionArray, (uint)allDimensionArray.Length);
        }
    }
}
