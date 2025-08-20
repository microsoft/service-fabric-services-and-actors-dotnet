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
    internal abstract class ServiceMeterProvider<TValueType> : IMeterProvider<TValueType>
    {
        readonly private IList<string> systemDimensionNames = new List<string>
        {
            "ReplicaOrInstanceId",
            "PartitionId",
            "ServiceTypeName",
            "ServiceName",
            "ApplicationName",
            "ApplicationTypeName"
        };
        readonly protected IList<string> systemDimensionValues = new List<string>();
        readonly protected IFabricMeterProvider fabricMeterProvider;

        private static Func<IFabricMeterProvider> createFabricMeterProvider = () => NativeRuntimeMethods.FabricCreateMeterProvider();

        private static Func<IFabricMeterProvider, string, string, string[], uint, IFabricMeter> createFabricMeter = (meterProvider, metricNamespace, metricName, dimensionNames, dimensionCount) => meterProvider.CreateMeter(metricNamespace, metricName, dimensionNames, dimensionCount);

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

        protected IFabricMeter CreateNativeMeter(string metricNamespace, string metricName, IList<string> additionalDimensions)
        {
            var allDimensionNames = new List<string>(systemDimensionNames.Concat(additionalDimensions));
            var dimensionNames = allDimensionNames.ToArray();

            return createFabricMeter(fabricMeterProvider, metricNamespace, metricName, dimensionNames, (uint)dimensionNames.Length);
        }

        public abstract IMeter<TValueType> CreateMeter(string metricNamespace, string name);
        public abstract IMeter1D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name);
        public abstract IMeter2D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name);
        public abstract IMeter3D<TValueType> CreateMeter(string metricNamespace, string name, string dimension1Name, string dimension2Name, string dimension3Name);
    }
}
