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
    abstract class ServiceMeterProvider<TValueType> : IMeterProvider<TValueType>
    {
        readonly IList<string> systemDimensionNames = new List<string>
        {
            nameof(ServiceContext.ReplicaOrInstanceId),
            nameof(ServiceContext.PartitionId),
            nameof(ServiceContext.ServiceTypeName),
            nameof(ServiceContext.ServiceName),
            nameof(ServiceContext.CodePackageActivationContext.ApplicationName),
            nameof(ServiceContext.CodePackageActivationContext.ApplicationTypeName)
        };
        protected readonly IList<string> systemDimensionValues = new List<string>();
        protected readonly IFabricMeterProvider fabricMeterProvider;

        private static Func<IFabricMeterProvider> createFabricMeterProvider = NativeTelemetry.FabricCreateMeterProvider;

        protected ServiceMeterProvider(ServiceContext serviceContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext), "Service context cannot be null.");
            }

            fabricMeterProvider = createFabricMeterProvider();

            systemDimensionValues.Add(serviceContext.ReplicaOrInstanceId.ToString());
            systemDimensionValues.Add(serviceContext.PartitionId.ToString());
            systemDimensionValues.Add(serviceContext.ServiceTypeName);
            systemDimensionValues.Add(serviceContext.ServiceName.ToString());
            systemDimensionValues.Add(serviceContext.CodePackageActivationContext.ApplicationName);
            systemDimensionValues.Add(serviceContext.CodePackageActivationContext.ApplicationTypeName);
        }

        protected IFabricMeter CreateNativeMeter(string metricNamespace, string metricName, IEnumerable<string> additionalDimensions)
        {
            List<string> allDimensionsList = new List<string>(systemDimensionNames.Count + additionalDimensions.Count());
            allDimensionsList.AddRange(systemDimensionNames);
            allDimensionsList.AddRange(additionalDimensions);

            string[] allDimensionNamesArray = allDimensionsList.ToArray();

            return fabricMeterProvider.CreateMeter(metricNamespace, metricName, (uint)allDimensionNamesArray.Length, allDimensionNamesArray);
        }

        public abstract IMeter<TValueType> CreateMeter(string metricNamespace, string name);
        public abstract IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name);
        public abstract IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name);
        public abstract IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name);
    }
}
