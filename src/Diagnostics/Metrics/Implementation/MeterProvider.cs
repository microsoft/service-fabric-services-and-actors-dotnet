// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Interop;
using System.Linq;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class MeterProvider<TValueType> : IMeterProvider<TValueType>
    {
        readonly IReadOnlyCollection<string> systemDimensionNames;
        protected readonly IReadOnlyCollection<string> systemDimensionValues;
        IFabricMeterProvider fabricMeterProvider;

        static Func<IFabricMeterProvider> createFabricMeterProvider = NativeTelemetry.FabricCreateMeterProvider;
        static Func<object, int> finalReleaseComObject = Utility.FinalReleaseComObject;

        protected MeterProvider(ServiceContext serviceContext = null)
        {
            fabricMeterProvider = createFabricMeterProvider();

            if (serviceContext != null)
            {
                systemDimensionNames =
                [
                    nameof(ServiceContext.PartitionId),
                    nameof(ServiceContext.ServiceTypeName),
                    nameof(ServiceContext.ServiceName),
                    nameof(ServiceContext.CodePackageActivationContext.ApplicationName),
                    nameof(ServiceContext.CodePackageActivationContext.ApplicationTypeName)
                ];

                systemDimensionValues =
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
                systemDimensionNames = [];
                systemDimensionValues = [];
            }
        }

        bool IsDisposed() => fabricMeterProvider == null;

        protected IFabricMeter CreateNativeMeter(string metricNamespace, string metricName, IEnumerable<string> additionalDimensions)
        {
            if (IsDisposed())
                throw new ObjectDisposedException(nameof(MeterProvider<>));

            var allDimensionsNameList = new List<string>(systemDimensionNames.Count + additionalDimensions.Count());

            allDimensionsNameList.AddRange(systemDimensionNames);
            allDimensionsNameList.AddRange(additionalDimensions);

            string[] allDimensions = [.. allDimensionsNameList];
            string[] fixedDimensionsValues = [.. systemDimensionValues];

            return fabricMeterProvider.CreateMeter(metricNamespace, metricName, (uint)allDimensions.Length, allDimensions, (uint)fixedDimensionsValues.Length, fixedDimensionsValues);
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
