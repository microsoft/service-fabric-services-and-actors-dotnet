// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class MeterProvider<TValueType> : IMeterProvider<TValueType>
    {
        readonly IEnumerable<string> systemDimensionNames;
        protected readonly IEnumerable<string> systemDimensionValues;
        protected readonly IFabricMeterProvider fabricMeterProvider;

        private static Func<IFabricMeterProvider> createFabricMeterProvider = NativeTelemetry.FabricCreateMeterProvider;

        protected MeterProvider(ServiceContext serviceContext = null)
        {
            fabricMeterProvider = createFabricMeterProvider();

            if (serviceContext != null)
            {
                this.systemDimensionNames = new[]
                {
                    nameof(ServiceContext.ReplicaOrInstanceId),
                    nameof(ServiceContext.PartitionId),
                    nameof(ServiceContext.ServiceTypeName),
                    nameof(ServiceContext.ServiceName),
                    nameof(ServiceContext.CodePackageActivationContext.ApplicationName),
                    nameof(ServiceContext.CodePackageActivationContext.ApplicationTypeName)
                };

                this.systemDimensionValues = new[]
                {
                    serviceContext.ReplicaOrInstanceId.ToString(),
                    serviceContext.PartitionId.ToString(),
                    serviceContext.ServiceTypeName,
                    serviceContext.ServiceName.ToString(),
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.CodePackageActivationContext.ApplicationTypeName
                };
            }
            else
            {
                this.systemDimensionNames = Enumerable.Empty<string>();
                this.systemDimensionValues = Enumerable.Empty<string>();
            }
        }

        protected IFabricMeter CreateNativeMeter(string metricNamespace, string metricName, IEnumerable<string> additionalDimensions)
        {
            var allDimensionsList = new List<string>(systemDimensionNames.Count() + additionalDimensions.Count());

            allDimensionsList.AddRange(systemDimensionNames);
            allDimensionsList.AddRange(additionalDimensions);

            var allDimensions = allDimensionsList.ToArray();
            return fabricMeterProvider.CreateMeter(metricNamespace, metricName, (uint)allDimensions.Length, allDimensions);
        }

        public abstract IMeter<TValueType> CreateMeter(string metricNamespace, string name);
        public abstract IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name);
        public abstract IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name);
        public abstract IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name);
    }
}
