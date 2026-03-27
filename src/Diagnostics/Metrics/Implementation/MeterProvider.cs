// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Interop;

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

        protected IFabricMeter CreateNativeMeter(string metricNamespace, string metricName, IEnumerable<string> variableDimensionNames)
        {
            if (IsDisposed())
                throw new ObjectDisposedException(nameof(MeterProvider<>));

            string[] allDimensionNames = [.. fixedDimensionNames, .. variableDimensionNames];
            string[] fixedDimensionsValues = [.. fixedDimensionValues];

            return fabricMeterProvider.CreateMeter(metricNamespace, metricName, (uint)allDimensionNames.Length, allDimensionNames, (uint)fixedDimensionsValues.Length, fixedDimensionsValues);
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
