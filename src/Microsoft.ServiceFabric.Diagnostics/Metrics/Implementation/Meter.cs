// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter
    {
        protected readonly string[] systemDimensionValues;
        protected readonly IFabricMeter fabricMeter;

        internal Meter(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues)
        {
            if (systemDimensionValues == null)
            {
                throw new ArgumentNullException(nameof(systemDimensionValues));
            }
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
            this.systemDimensionValues = systemDimensionValues.ToArray();
        }

        protected long ConvertTimeSpanToLong(TimeSpan value)
        {
            return (long)Math.Round(value.TotalMilliseconds);
        }

        protected void Record(long value, params string[] customDimensions)
        {
            var allDimensionArray = new string[systemDimensionValues.Length + customDimensions.Length];
            systemDimensionValues.CopyTo(allDimensionArray, 0);
            customDimensions.CopyTo(allDimensionArray, systemDimensionValues.Length);
            fabricMeter.Record(value, (uint)allDimensionArray.Length, allDimensionArray);
        }
    }
}
