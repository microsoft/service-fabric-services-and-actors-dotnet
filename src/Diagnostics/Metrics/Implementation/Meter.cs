// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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
            if (customDimensionCount < 0 || customDimensionCount > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(customDimensionCount));
            }
            if (customDimension3 == null && customDimensionCount == 3)
            {
                throw new ArgumentException(nameof(customDimension3));
            }
            if (customDimension2 == null && customDimensionCount >= 2)
            {
                throw new ArgumentException(nameof(customDimension2));
            }
            if (customDimension1 == null && customDimensionCount >= 1)
            {
                throw new ArgumentException(nameof(customDimension1));
            }

            recordAction.Invoke(value, customDimensionCount, customDimension1, customDimension2, customDimension3);
        }

        unsafe private void RecordViaNative(long value, int customDimensionCount, string customDimension1, string customDimension2, string customDimension3)
        {
            int totalDimensionCount = systemDimensionValues.Length + customDimensionCount;

            GCHandle* allDimensionPins = stackalloc GCHandle[totalDimensionCount];
            IntPtr* allDimensionValuesPointers = stackalloc IntPtr[totalDimensionCount];

            try
            {
                for (int i = 0; i < systemDimensionValues.Length; i++)
                {
                    allDimensionPins[i] = GCHandle.Alloc(systemDimensionValues[i], GCHandleType.Pinned);
                }

                if (customDimensionCount > 0)
                    allDimensionPins[systemDimensionValues.Length] = GCHandle.Alloc(customDimension1, GCHandleType.Pinned);

                if (customDimensionCount > 1)
                    allDimensionPins[systemDimensionValues.Length + 1] = GCHandle.Alloc(customDimension2, GCHandleType.Pinned);

                if (customDimensionCount > 2)
                    allDimensionPins[systemDimensionValues.Length + 2] = GCHandle.Alloc(customDimension3, GCHandleType.Pinned);


                for (int i = 0; i < totalDimensionCount; i++)
                {
                    // for strings, AddrOfPinnedObject() returns a pointer to the first character - https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.gchandle.addrofpinnedobject
                    allDimensionValuesPointers[i] = allDimensionPins[i].AddrOfPinnedObject();
                }

                fabricMeter.Record(value, (uint)totalDimensionCount, (IntPtr)allDimensionValuesPointers);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                for (int i = 0; i < totalDimensionCount; i++)
                {
                    if (allDimensionPins[i].IsAllocated)
                        allDimensionPins[i].Free();
                }
            }
        }
    }
}
