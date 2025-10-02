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
        private readonly Action<long, int, string, string, string> recordAction;

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
            this.recordAction = RecordViaNative;
        }

        protected long ConvertTimeSpanToLong(TimeSpan value)
        {
            return (long)Math.Round(value.TotalMilliseconds);
        }

        protected void Record(long value, int customDimensionCount = 0, string customDimension1 = null, string customDimension2 = null, string customDimension3 = null)
        {
            recordAction.Invoke(value, customDimensionCount, customDimension1, customDimension2, customDimension3);
        }

        private void RecordViaNative(long value, int customDimensionCount, string customDimension1, string customDimension2, string customDimension3)
        {
            if (customDimensionCount < 0 || customDimensionCount > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(customDimensionCount));
            }

            var allDimensionArray = new string[systemDimensionValues.Length + customDimensionCount];
            systemDimensionValues.CopyTo(allDimensionArray, 0);

            if (customDimensionCount > 0)
            {
                allDimensionArray[systemDimensionValues.Length] = customDimension1;
            }
            if (customDimensionCount > 1)
            {
                allDimensionArray[systemDimensionValues.Length + 1] = customDimension2;
            }
            if (customDimensionCount > 2)
            {
                allDimensionArray[systemDimensionValues.Length + 2] = customDimension3;
            }

            fabricMeter.Record(value, (uint)allDimensionArray.Length, allDimensionArray);
        }
    }
}
