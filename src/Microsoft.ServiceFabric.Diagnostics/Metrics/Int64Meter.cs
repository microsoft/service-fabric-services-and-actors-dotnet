// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    /// <summary>
    /// Implementation of <see cref="IMeter{TValueType}"/> for long integer metrics.
    /// Records integer telemetry values with system dimensions.
    /// </summary>
    sealed class Int64Meter : MeterBase, IMeter<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int64Meter"/> class.
        /// </summary>
        /// <param name="fabricMeter">The native fabric meter implementation.</param>
        /// <param name="systemDimensionValues">System dimension values that will be included with all recorded metrics.</param>
        internal Int64Meter(IFabricMeter fabricMeter, IList<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }

        /// <summary>
        /// Records a long integer telemetry value.
        /// The metric will be recorded with system dimensions.
        /// </summary>
        /// <param name="value">The integer value to record.</param>
        public void Record(long value)
        {
            string[] allDimensionArray = systemDimensionValues.ToArray();
            fabricMeter.Record(value, allDimensionArray, (uint)allDimensionArray.Length);
        }
    }
}
