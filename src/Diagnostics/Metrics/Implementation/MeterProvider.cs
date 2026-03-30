// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class MeterProvider<TValueType> : IMeterProvider<TValueType>
    {
        readonly IReadOnlyCollection<string> fixedDimensionNames;
        protected readonly IReadOnlyCollection<string> fixedDimensionValues;
        IFabricMeterProvider fabricMeterProvider;

        static Func<IFabricMeterProvider> createFabricMeterProvider = NativeTelemetry.FabricCreateMeterProvider;
        static Func<object, int> finalReleaseComObject = Utility.FinalReleaseComObject;

        protected MeterProvider(ServiceContext serviceContext = null)
        {
            fabricMeterProvider = createFabricMeterProvider();

            if (serviceContext != null)
            {
                fixedDimensionNames =
                [
                    nameof(ServiceContext.PartitionId),
                    nameof(ServiceContext.ServiceTypeName),
                    nameof(ServiceContext.ServiceName),
                    nameof(ServiceContext.CodePackageActivationContext.ApplicationName),
                    nameof(ServiceContext.CodePackageActivationContext.ApplicationTypeName)
                ];

                fixedDimensionValues =
                [
                    serviceContext.PartitionId.ToString(),
                    serviceContext.ServiceTypeName,
                    serviceContext.ServiceName.ToString(),
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.CodePackageActivationContext.ApplicationTypeName
                ];
            }
            else
            {
                fixedDimensionNames = [];
                fixedDimensionValues = [];
            }
        }

        bool IsDisposed() => fabricMeterProvider == null;

        protected unsafe IFabricMeter CreateNativeMeter(string metricNamespace, string metricName, IEnumerable<string> variableDimensionNames)
        {
            if (IsDisposed())
                throw new ObjectDisposedException(nameof(MeterProvider<>));

            string[] allDimensionNames = [.. fixedDimensionNames, .. variableDimensionNames];
            string[] fixedDimValues = [.. fixedDimensionValues];
            int totalPins = 2 + allDimensionNames.Length + fixedDimValues.Length;

            GCHandle* pins = stackalloc GCHandle[totalPins];
            IntPtr* dimensionNamePtrs = stackalloc IntPtr[allDimensionNames.Length];
            IntPtr* fixedValuePtrs = stackalloc IntPtr[fixedDimValues.Length];

            try
            {
                int p = 0;
                pins[p++] = GCHandle.Alloc(metricNamespace, GCHandleType.Pinned);
                pins[p++] = GCHandle.Alloc(metricName, GCHandleType.Pinned);

                for (int i = 0; i < allDimensionNames.Length; i++)
                    pins[p++] = GCHandle.Alloc(allDimensionNames[i], GCHandleType.Pinned);

                for (int i = 0; i < fixedDimValues.Length; i++)
                    pins[p++] = GCHandle.Alloc(fixedDimValues[i], GCHandleType.Pinned);

                for (int i = 0; i < allDimensionNames.Length; i++)
                    dimensionNamePtrs[i] = pins[2 + i].AddrOfPinnedObject();

                for (int i = 0; i < fixedDimValues.Length; i++)
                    fixedValuePtrs[i] = pins[2 + allDimensionNames.Length + i].AddrOfPinnedObject();

                FABRIC_METER_DESCRIPTION description;
                description.Namespace = pins[0].AddrOfPinnedObject();
                description.Name = pins[1].AddrOfPinnedObject();
                description.TotalDimensionsCount = (uint)allDimensionNames.Length;
                description.DimensionNames = (IntPtr)dimensionNamePtrs;
                description.FixedDimensionCount = (uint)fixedDimValues.Length;
                description.FixedDimensionValues = (IntPtr)fixedValuePtrs;
                description.Reserved = IntPtr.Zero;

                return fabricMeterProvider.CreateMeter((IntPtr)(&description));
            }
            finally
            {
                Interop.Free(pins, totalPins);
            }
        }

        public void Dispose()
        {
            if (!IsDisposed())
            {
                finalReleaseComObject(fabricMeterProvider);
                fabricMeterProvider = null;
            }
        }

        public abstract IMeter<TValueType> CreateMeter(string metricNamespace, string name);
        public abstract IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name);
        public abstract IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name);
        public abstract IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name);
    }
}
