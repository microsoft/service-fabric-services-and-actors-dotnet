// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class Int64Meter2D : Meter, IMeter2D<long>
    {
        internal Int64Meter2D(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        public void Record(long value, string dimension1, string dimension2)
        {
            var systemDimensionsList = (List<string>)systemDimensionValues;
            var allDimensionArray = new string[systemDimensionsList.Count + 2];

            systemDimensionsList.CopyTo(allDimensionArray, 0);
            allDimensionArray[allDimensionArray.Length - 2] = dimension1;
            allDimensionArray[allDimensionArray.Length - 1] = dimension2;

            fabricMeter.Record(value, (uint)allDimensionArray.Length, allDimensionArray);
        }
    }
}
