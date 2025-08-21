// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    /// <summary>
    /// Implementation of <see cref="IMeter3D{TValueType}"/> for long integer metrics.
    /// Records integer telemetry values with system dimensions and three additional dimensions.
    /// </summary>
    sealed class Int64Meter3D : MeterBase, IMeter3D<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int64Meter3D"/> class.
        /// </summary>
        /// <param name="fabricMeter">The native fabric meter implementation.</param>
        /// <param name="systemDimensionValues">System dimension values that will be included with all recorded metrics.</param>
        internal Int64Meter3D(IFabricMeter fabricMeter, IList<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        /// <summary>
        /// Records a long integer telemetry value with three additional dimensions.
        /// The metric will be recorded with system dimensions plus the specified additional dimensions.
        /// </summary>
        /// <param name="value">The integer value to record.</param>
        /// <param name="dimension1">The first additional dimension value.</param>
        /// <param name="dimension2">The second additional dimension value.</param>
        /// <param name="dimension3">The third additional dimension value.</param>
        public void Record(long value, string dimension1, string dimension2, string dimension3)
        {
            var allDimensionsList = new List<string>(systemDimensionValues)
            {
                dimension1,
                dimension2,
                dimension3
            };
            string[] allDimensionArray = allDimensionsList.ToArray();
            fabricMeter.Record(value, (uint)allDimensionArray.Length, allDimensionArray);
        }
    }
}
