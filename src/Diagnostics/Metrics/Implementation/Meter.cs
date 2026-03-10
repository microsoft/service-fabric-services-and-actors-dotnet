// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter : IDisposable
    {
        protected readonly IFabricMeter fabricMeter;

        protected readonly IReadOnlyCollection<string> systemDimensionValues;

        internal Meter(IFabricMeter fabricMeter, IReadOnlyCollection<string> systemDimensionValues)
        {
            _ = systemDimensionValues ?? throw new ArgumentNullException(nameof(systemDimensionValues));
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
            this.systemDimensionValues = systemDimensionValues;
        }

        /// <summary>
        /// Releases the native COM resources held by this meter.
        /// </summary>
        public void Dispose()
        {
            // TODO: Release the fabricMeter COM object
        }

        protected void Record(long value)
        {
            RecordViaNative(value, 0, null, null, null);
        }

        protected long ConvertTimeSpanToLong(TimeSpan value)
        {
            return (long)Math.Round(value.TotalMilliseconds);
        }

        unsafe protected void RecordViaNative(long value, int customDimensionCount, string customDimension1, string customDimension2, string customDimension3)
        {
            if (customDimensionCount < 0 || customDimensionCount > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(customDimensionCount));
            }

            int totalDimensionCount = systemDimensionValues.Count + customDimensionCount;

            GCHandle* allDimensionPins = stackalloc GCHandle[totalDimensionCount];
            IntPtr* allDimensionValuesPointers = stackalloc IntPtr[totalDimensionCount];

            try
            {
                int i = 0;
                foreach (string systemDimensionValue in systemDimensionValues)
                {
                    allDimensionPins[i++] = GCHandle.Alloc(systemDimensionValue, GCHandleType.Pinned);
                }

                if (customDimensionCount > 0)
                    allDimensionPins[i++] = GCHandle.Alloc(customDimension1, GCHandleType.Pinned);

                if (customDimensionCount > 1)
                    allDimensionPins[i++] = GCHandle.Alloc(customDimension2, GCHandleType.Pinned);

                if (customDimensionCount > 2)
                    allDimensionPins[i++] = GCHandle.Alloc(customDimension3, GCHandleType.Pinned);


                for (i = 0; i < totalDimensionCount; i++)
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
