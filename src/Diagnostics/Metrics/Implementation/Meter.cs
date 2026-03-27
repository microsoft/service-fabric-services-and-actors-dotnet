// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter : IDisposable
    {
        IFabricMeter fabricMeter;

        static Func<object, int> finalReleaseComObject = Utility.FinalReleaseComObject;

        internal Meter(IFabricMeter fabricMeter)
        {
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
        }

        public void Dispose()
        {
            if (!IsDisposed())
            {
                finalReleaseComObject(fabricMeter);
                fabricMeter = null;
            }
        }

        protected void Record(long value) => RecordViaNative(value, 0, null, null, null);

        protected long ConvertTimeSpanToLong(TimeSpan value) => (long)Math.Round(value.TotalMilliseconds);

        bool IsDisposed() => fabricMeter == null;

        protected unsafe void RecordViaNative(long value, int variableDimensionCount, string dimension1Value, string dimension2Value, string dimension3Value)
        {
            if (IsDisposed())
                throw new ObjectDisposedException(nameof(Meter));
            if (variableDimensionCount < 0 || variableDimensionCount > 3)
                throw new ArgumentOutOfRangeException(nameof(variableDimensionCount));

            GCHandle* dimensionPins = stackalloc GCHandle[variableDimensionCount];
            IntPtr* dimensionValuesPointers = stackalloc IntPtr[variableDimensionCount];

            try
            {
                int i = 0;

                if (variableDimensionCount > 0)
                    dimensionPins[i++] = GCHandle.Alloc(dimension1Value, GCHandleType.Pinned);

                if (variableDimensionCount > 1)
                    dimensionPins[i++] = GCHandle.Alloc(dimension2Value, GCHandleType.Pinned);

                if (variableDimensionCount > 2)
                    dimensionPins[i++] = GCHandle.Alloc(dimension3Value, GCHandleType.Pinned);


                for (i = 0; i < variableDimensionCount; i++)
                {
                    // for strings, AddrOfPinnedObject() returns a pointer to the first character - https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.gchandle.addrofpinnedobject
                    dimensionValuesPointers[i] = dimensionPins[i].AddrOfPinnedObject();
                }

                fabricMeter.Record(value, (uint)variableDimensionCount, (IntPtr)dimensionValuesPointers);
            }
            finally
            {
                for (int i = 0; i < variableDimensionCount; i++)
                {
                    if (dimensionPins[i].IsAllocated)
                        dimensionPins[i].Free();
                }
            }
        }
    }
}
