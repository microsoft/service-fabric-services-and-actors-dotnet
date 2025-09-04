// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class Int64Meter1D : Meter, IMeter1D<long>
    {
        internal Int64Meter1D(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        public void Record(long value, string dimension1)
        {
            var allDimensionArray = new string[systemDimensionValues.Count() + 1];

            systemDimensionValues.CopyTo(allDimensionArray, 0);
            allDimensionArray[allDimensionArray.Length - 1] = dimension1;

            fabricMeter.Record(value, (uint)allDimensionArray.Length, allDimensionArray);
        }
    }
}
